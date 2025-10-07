
using Azure.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace BioLinker.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _senderName;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;

            // Lấy cấu hình từ appsettings.json
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUser = _configuration["EmailSettings:SenderEmail"] ?? "";
            _smtpPass = _configuration["EmailSettings:AppPassword"] ?? "";
            _senderName = _configuration["EmailSettings:SenderName"] ?? "BioLinker Team";
        }
        public async Task<string> SendConfirmationEmailAsync(string email, string code)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<title>Email Verification - BioLinker</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; background-color: #f4f7fb; margin: 0; padding: 0; }");
            sb.AppendLine(".container { background-color: #ffffff; max-width: 600px; margin: 40px auto; border-radius: 8px; box-shadow: 0px 3px 12px rgba(0,0,0,0.1); padding: 30px; }");
            sb.AppendLine(".header { text-align: center; background: linear-gradient(90deg, #6A5ACD, #00BFFF); color: white; padding: 14px 0; border-radius: 8px 8px 0 0; }");
            sb.AppendLine(".content { padding: 20px; text-align: center; }");
            sb.AppendLine(".code { display: inline-block; font-size: 28px; letter-spacing: 5px; font-weight: bold; background-color: #f1f1f1; padding: 12px 30px; border-radius: 6px; margin: 15px 0; }");
            sb.AppendLine(".footer { margin-top: 30px; font-size: 14px; color: #555; text-align: center; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"container\">");
            sb.AppendLine("<div class=\"header\"><h2>Verify Your Email</h2></div>");
            sb.AppendLine("<div class=\"content\">");
            sb.AppendLine("<p>Hello 👋,</p>");
            sb.AppendLine("<p>Thank you for registering with <strong>BioLinker</strong> 🎉</p>");
            sb.AppendLine("<p>Please use the verification code below to activate your account:</p>");
            sb.AppendLine($"<div class=\"code\">{code}</div>");
            sb.AppendLine("<p>This code will expire in <strong>10 minutes</strong>.</p>");
            sb.AppendLine("<p>If you didn’t sign up for BioLinker, you can safely ignore this email.</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"footer\">— The BioLinker Team</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            await SendEmailAsync(email, "Verify your BioLinker Account", sb.ToString());
            return "Verification email sent.";
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _smtpUser));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
