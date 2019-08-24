using Certification_System.ServicesInterfaces.Models;
using System.Threading.Tasks;

namespace Certification_System.ServicesInterfaces
{
    public interface IEmailSender
    {
        EmailMessageDto GenerateEmailMessage(string receiverEmailAddress, string receiverName, string messageType, string link);
        Task SendEmailAsync(EmailMessageDto emailMessage);
    }
}
