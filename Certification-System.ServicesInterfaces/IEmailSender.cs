using System.Threading.Tasks;

namespace Certification_System.ServicesInterfaces.IEmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
