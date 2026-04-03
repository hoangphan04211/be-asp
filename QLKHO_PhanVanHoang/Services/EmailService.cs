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
            var emailHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var emailPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var emailUser = _config["Smtp:User"] ?? "";
            var emailPass = _config["Smtp:Pass"] ?? "";
            var senderEmail = _config["Smtp:SenderEmail"] ?? emailUser;
            var senderName = _config["Smtp:SenderName"] ?? "Hệ thống WMS";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
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
