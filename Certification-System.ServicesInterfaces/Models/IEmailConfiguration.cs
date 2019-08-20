namespace Certification_System.ServicesInterfaces.Models
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SenderName { get; set; }
        string SmtpPassword { get; set; }
    }
}
