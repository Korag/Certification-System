namespace Certification_System.ServicesInterfaces.Models
{
    public class EmailMessageDto
    {
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverName { get; set; }

        public string Subject { get; set; }
        public string BodyMessage { get; set; }
    }
}
