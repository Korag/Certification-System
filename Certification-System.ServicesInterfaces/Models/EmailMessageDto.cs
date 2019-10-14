namespace Certification_System.ServicesInterfaces.Models
{
    public class EmailMessageDto
    {
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverName { get; set; }

        public string Subject { get; set; }

        public string Header { get; set; }
        public string BodyMessage { get; set; }

        public string Link { get; set; }
        public string LinkText { get; set; }

        public string ObjectClassifier { get; set; }
        public string Indexer { get; set; }
    }
}
