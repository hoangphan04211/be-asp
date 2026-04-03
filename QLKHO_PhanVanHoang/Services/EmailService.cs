using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Cấu hình mẫu (Mock / Dev log). Thực tế lấy từ appsettings.json SMTP
            var emailHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var emailPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var emailUser = _config["Smtp:User"] ?? "your-email@gmail.com";
            var emailPass = _config["Smtp:Pass"] ?? "your-app-password";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hệ thống WMS", "no-reply@wms.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            try {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; 
                
                // Mở kết nối tới SMTP Server
                await client.ConnectAsync(emailHost, emailPort, MailKit.Security.SecureSocketOptions.StartTls);
                
                // Xác thực tài khoản
                await client.AuthenticateAsync(emailUser, emailPass);
                
                // Gửi email
                await client.SendAsync(message);
                
                // Ngắt kết nối
                await client.DisconnectAsync(true);
                
                Serilog.Log.Information($"[Email Sent] Đã gửi Email thành công tới {toEmail}. Tiêu đề: {subject}");
            } catch (System.Exception ex) {
                Serilog.Log.Error($"Lỗi gửi Email: {ex.Message}");
            }
        }
    }
}
