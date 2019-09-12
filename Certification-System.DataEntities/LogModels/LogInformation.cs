using System;

namespace Certification_System.Entities
{
    public class LogInformation
    {
        public string ChangeAuthorIdentificator { get; set; }

        public string ChangeAuthorEmail { get; set; }
        public string ChangeAuthorFirstName { get; set; }
        public string ChangeAuthorLastName { get; set; }

        public string TypeOfAction { get; set; }
        public DateTime DateTime { get; set; }
    }
}
