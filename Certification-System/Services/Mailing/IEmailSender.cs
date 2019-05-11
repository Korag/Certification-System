using System.Threading.Tasks;

namespace Certification_System.Mailing
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
