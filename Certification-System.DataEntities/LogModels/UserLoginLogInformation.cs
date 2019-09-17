using System;

namespace Certification_System.Entities
{
    public class UserLoginLogInformation
    {
        public string UserIdentificator { get; set; }

        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        public string ActionResult { get; set; }

        public string IpAddress { get; set; }
        public DateTime DateTime { get; set; }

    }
}
