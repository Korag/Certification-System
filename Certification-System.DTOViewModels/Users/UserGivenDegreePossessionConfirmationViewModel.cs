using System;
using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class UserGivenDegreePossessionConfirmationViewModel
    {
        public string UserIdentificator { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] QRCode;

        public string GivenDegreeIdentificator { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ICollection<string> RequiredCertificatesNames { get; set; }
        public ICollection<string> RequiredDegreesNames { get; set; }
        public ICollection<string> Conditions { get; set; }

        public string DegreeIdentificator { get; set; }
        public string DegreeName { get; set; }
        public string DegreeDescription { get; set; }
        public ICollection<string> Branches { get; set; }
    }
}
