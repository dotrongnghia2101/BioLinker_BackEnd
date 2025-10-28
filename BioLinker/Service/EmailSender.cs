
using Azure.Core;
using MailKit.Net.Smtp;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace BioLinker.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailSender(IConfiguration configuration)
        {
            System.Net.ServicePointManager.SecurityProtocol =
            System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

            _configuration = configuration;
            _apiKey = _configuration["Brevo:ApiKey"] ?? throw new Exception("Brevo ApiKey missing");
            _senderEmail = _configuration["Brevo:SenderEmail"] ?? "noreply@biolinker.app";
            _senderName = _configuration["Brevo:SenderName"] ?? "BioLinker";

            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
                Configuration.Default.ApiKey.Add("api-key", _apiKey);
            else
                Configuration.Default.ApiKey["api-key"] = _apiKey;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiInstance = new TransactionalEmailsApi();
            var sender = new SendSmtpEmailSender(_senderName, _senderEmail);
            var to = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(email) };
            var emailData = new SendSmtpEmail(
                 sender: sender,
                 to: to,
                 subject: subject,
                 htmlContent: htmlMessage
             );

            await apiInstance.SendTransacEmailAsync(emailData);
        }

        public async Task<string> SendConfirmationEmailAsync(string email, string code)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head><meta charset=\"UTF-8\"><title>Email Verification</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; background-color: #f4f7fb; }");
            sb.AppendLine(".container { background-color: #fff; max-width: 600px; margin: 40px auto; border-radius: 8px; box-shadow: 0px 3px 12px rgba(0,0,0,0.1); padding: 30px; }");
            sb.AppendLine(".header { text-align: center; background: linear-gradient(90deg, #6A5ACD, #00BFFF); color: white; padding: 14px; border-radius: 8px 8px 0 0; }");
            sb.AppendLine(".code { font-size: 28px; letter-spacing: 5px; background-color: #f1f1f1; padding: 10px 20px; display: inline-block; border-radius: 6px; margin: 10px 0; }");
            sb.AppendLine(".footer { margin-top: 30px; font-size: 14px; color: #555; text-align: center; }");
            sb.AppendLine("</style></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<div class='header'><h2>Verify Your Email</h2></div>");
            sb.AppendLine("<p>Hello 👋,</p>");
            sb.AppendLine("<p>Thank you for registering with <strong>BioLinker</strong> 🎉</p>");
            sb.AppendLine("<p>Use the code below to verify your email:</p>");
            sb.AppendLine($"<div class='code'>{code}</div>");
            sb.AppendLine("<p>This code will expire in <strong>10 minutes</strong>.</p>");
            sb.AppendLine("<p>If you didn’t sign up for BioLinker, please ignore this email.</p>");
            sb.AppendLine("<div class='footer'>— The BioLinker Team</div>");
            sb.AppendLine("</div></body></html>");

            await SendEmailAsync(email, "Verify your BioLinker Account", sb.ToString());
            return "Verification email sent.";
        }



        public async Task SendUpgradeToProEmailAsync(string email, string userName, DateTime expireAt)
        {
            string subject = "🎉 Chúc mừng! Tài khoản BioLinker của bạn đã được nâng cấp lên Pro";

            var html = $@"
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{ font-family: Arial; background-color: #f7f8fb; }}
            .container {{
                background-color: #fff;
                max-width: 600px;
                margin: 40px auto;
                border-radius: 10px;
                padding: 25px;
                box-shadow: 0 3px 12px rgba(0,0,0,0.1);
            }}
            .header {{
                background: linear-gradient(90deg,#6A5ACD,#00BFFF);
                color: white;
                padding: 15px;
                border-radius: 10px 10px 0 0;
                text-align: center;
            }}
            .content {{ margin-top: 20px; font-size: 16px; color: #333; }}
            .highlight {{ font-weight: bold; color: #007BFF; }}
            .footer {{ margin-top: 30px; font-size: 14px; color: #555; text-align: center; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'><h2>BioLinker Pro Activated 🚀</h2></div>
            <div class='content'>
                <p>Xin chào <strong>{userName}</strong>,</p>
                <p>Bạn đã được nâng cấp lên gói <span class='highlight'>BioLinker Pro</span>.</p>
                <p>Thời hạn sử dụng: <strong>đến ngày {expireAt:dd/MM/yyyy}</strong>.</p>
                <p>Hãy tận hưởng các tính năng cao cấp như tùy chỉnh giao diện, template độc quyền và phân tích nâng cao 💡</p>
            </div>
            <div class='footer'>— Đội ngũ BioLinker</div>
        </div>
    </body>
    </html>";

            await SendEmailAsync(email, subject, html);
        }

        public async Task SendPaymentSuccessEmailAsync(string email, string planName, DateTime expireAt)
        {
            string subject = $"🎉 Thanh toán thành công - Nâng cấp gói {planName}";

            var html = $@"
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f7fb;
            }}
            .container {{
                background-color: #fff;
                max-width: 600px;
                margin: 40px auto;
                border-radius: 8px;
                box-shadow: 0px 3px 12px rgba(0,0,0,0.1);
                padding: 30px;
            }}
            .header {{
                text-align: center;
                background: linear-gradient(90deg, #6A5ACD, #00BFFF);
                color: white;
                padding: 14px;
                border-radius: 8px 8px 0 0;
            }}
            .content p {{
                font-size: 16px;
                color: #333;
                line-height: 1.5;
            }}
            .highlight {{
                color: #007bff;
                font-weight: bold;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 14px;
                color: #555;
                text-align: center;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'><h2>🎉 Thanh toán thành công</h2></div>
            <div class='content'>
                <p>Xin chào,</p>
                <p>Bạn đã thanh toán thành công và được nâng cấp lên gói <span class='highlight'>{planName}</span>.</p>
                <p>Thời hạn gói của bạn đến hết ngày <strong>{expireAt:dd/MM/yyyy}</strong>.</p>
                <p>Chúc bạn tận hưởng những tính năng cao cấp của BioLinker!</p>
            </div>
            <div class='footer'>— Đội ngũ BioLinker 💙</div>
        </div>
    </body>
    </html>";

            await SendEmailAsync(email, subject, html);
        }
    }
    }
