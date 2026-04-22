using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QLKHO_PhanVanHoang.Data;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using QLKHO_PhanVanHoang.Jobs;
using QLKHO_PhanVanHoang.Middlewares;
using QLKHO_PhanVanHoang.Hubs;
using Hangfire;
using Hangfire.MySql;
using System.Reflection;
using QLKHO_PhanVanHoang.Helpers;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace QLKHO_PhanVanHoang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== 1. THÊM CÁC DỊCH VỤ VÀO CONTAINER =====
            
            // Cấu hình Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/qlykho-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.Host.UseSerilog();

            // Thêm Controllers và cấu hình FluentValidation
            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            // Tự động tìm và đăng ký tất cả Validator từ Assembly
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Đăng ký HttpContextAccessor để lấy thông tin user đăng nhập
            builder.Services.AddHttpContextAccessor();

            // Cấu hình Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "QLKHO_PhanVanHoang API",
                    Version = "v1",
                    Description = "API quản lý kho thông minh - Phan Văn Hoàng",
                    Contact = new OpenApiContact
                    {
                        Name = "Phan Văn Hoàng",
                        Email = "phan21828@gmail.com"
                    }
                });

                // Add JWT Security for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token (without 'Bearer ' prefix)",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // === QUAN TRỌNG: Đăng ký DbContext với SQL Server ===
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("CRITICAL: Connection string 'DefaultConnection' is empty! Check your Environment Variables.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31)), 
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure())
                       // Enable logging chi tiết (chỉ dùng khi phát triển)
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, LogLevel.Information));

            // Thêm CORS (cho phép frontend gọi API)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true) // Cho phép tất cả các nguồn (Vercel, Localhost...)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Bắt buộc cho SignalR
                });
            });

            // Thêm AutoMapper (sẽ tạo sau)
            builder.Services.AddAutoMapper(typeof(Program));

            // Đăng ký UnitOfWork và Generic Repository vào Dependency Injection
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký các Services
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IInboundService, InboundService>();
            builder.Services.AddScoped<IOutboundService, OutboundService>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<ICountingService, CountingService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, CloudinaryService>();
            builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();

            // Cloudinary Settings
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            // Cấu hình SignalR
            builder.Services.AddSignalR();

            // Cấu hình Hangfire
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
                {
                    TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true
                })));
            builder.Services.AddHangfireServer();

            // Cấu hình JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if(jwtKey != null) {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtIssuer,
                            ValidAudience = jwtAudience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                            NameClaimType = System.Security.Claims.ClaimTypes.Name
                        };
                    });
            }

            // Phân quyền
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Sử dụng Global Exception Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            // ===== 2. TỰ ĐỘNG TẠO DATABASE NẾU CHƯA CÓ =====
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    // Tự động chạy Migration để cập nhật database
                    dbContext.Database.Migrate();
                    Console.WriteLine("✓ Đã cập nhật database thông qua Migration thành công!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Lỗi khi kết nối database: {ex.Message}");
                    Console.WriteLine("Vui lòng kiểm tra:");
                    Console.WriteLine("1. MySQL trên cPanel đã bật và cho phép 'Remote MySQL' (%) chưa?");
                    // An toàn: Không in toàn bộ connection string nhưng in các phần quan trọng để debug
                    if (!string.IsNullOrEmpty(connectionString)) {
                         var parts = connectionString.Split(';');
                         var server = parts.FirstOrDefault(p => p.StartsWith("Server=", StringComparison.OrdinalIgnoreCase));
                         var db = parts.FirstOrDefault(p => p.StartsWith("Database=", StringComparison.OrdinalIgnoreCase));
                         Console.WriteLine($"2. Kiểm tra thông số: {server}, {db}");
                    }
                    Console.WriteLine("3. User/Password trong Connection String có đúng không?");
                }
            }

            // ===== 3. CẤU HÌNH PIPELINE XỬ LÝ REQUEST =====

            // Cấu hình Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QLKHO API V1");
                c.RoutePrefix = string.Empty; // Swagger ở trang chủ
            });

            // Sử dụng CORS
            app.UseCors("AllowAll");

            // Phục vụ file tĩnh (ảnh sản phẩm)
            app.UseStaticFiles();

            // Chuyển hướng HTTP sang HTTPS
            app.UseHttpsRedirection();

            // Phân quyền (sẽ thêm sau khi có Authentication)
            app.UseAuthentication();
            app.UseAuthorization();

            // Cấu hình Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire");

            // Lập lịch Job ngầm mỗi đêm 23h59
            RecurringJob.AddOrUpdate<InventoryAlertJob>(
                "daily-inventory-check",
                job => job.CheckExpiryAndLowStockAsync(),
                Cron.Daily(23, 59));

            // Map các hubs SignalR
            app.MapHub<NotificationHub>("/notificationHub");

            // Map các controllers
            app.MapControllers();

            // Chạy ứng dụng
            app.Run();
        }
    }
}