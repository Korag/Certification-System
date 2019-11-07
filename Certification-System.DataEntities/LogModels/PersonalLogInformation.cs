﻿using System;

namespace Certification_System.Entities
{
    public class PersonalLogInformation
    {
        public string ChangeAuthorIdentificator { get; set; }

        public string ChangeAuthorEmail { get; set; }
        public string ChangeAuthorFirstName { get; set; }
        public string ChangeAuthorLastName { get; set; }

        public string DescriptionOfAction { get; set; }
        public string AdditionalInfo { get; set; }
        public string ActionName { get; set; }

        public string IpAddress { get; set; }
        public DateTime DateOfLogCreation { get; set; }
    }
}
