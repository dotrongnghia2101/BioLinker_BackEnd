using System.Threading.Tasks;

namespace BioLinker.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task<string> SendConfirmationEmailAsync(string email, string code);
    }
}
