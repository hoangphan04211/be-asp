using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QLKHO_PhanVanHoang.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("debug_error.log", $"\n[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n");
                _logger.LogError(ex, "Lỗi hệ thống: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            List<string>? errors = null;

            if (exception is ArgumentException || exception.Message.Contains("Không tìm thấy") || exception.Message.Contains("tồn tại"))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is DbUpdateConcurrencyException)
            {
                statusCode = (int)HttpStatusCode.Conflict;
                message = "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang.";
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.FailureResult(message, errors);
            
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }
    }
}
