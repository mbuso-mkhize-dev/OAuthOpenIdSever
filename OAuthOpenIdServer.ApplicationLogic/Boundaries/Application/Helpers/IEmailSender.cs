using System.Threading.Tasks;

namespace OAuthOpenIdServer.ApplicationLogic.Boundaries.Application.Services
{
    public interface IEmailSender
    {
        void SendAccountRecoveryEmail(string callbackUrl, string toEmail);

        Task SendAccountRecoveryEmailAsync(string callbackUrl, string toEmail);

        void SendAccountConfirmationEmail(string callbackUrl, string toEmail);

        Task SendAccountConfirmationEmailAsync(string callbackUrl, string toEmail);

        void SendFeedbackEmail(string fromEmail, string fromName, string toEmail, string subject, string body);
    }
}