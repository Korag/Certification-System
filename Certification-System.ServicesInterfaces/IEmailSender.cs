using Certification_System.ServicesInterfaces.Models;
using System.Threading.Tasks;

namespace Certification_System.ServicesInterfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessageDto emailMessage);
    }
}
