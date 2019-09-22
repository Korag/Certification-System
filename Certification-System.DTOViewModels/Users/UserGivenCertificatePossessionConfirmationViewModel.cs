using System;
using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class UserGivenCertificatePossessionConfirmationViewModel
    {
        public string UserIdentificator { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] QRCode;

        public string GivenCertificateIdentificator { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string CertificateIdentificator { get; set; }
        public string CertificateName { get; set; }
        public string CertificateDescription { get; set; }
        public ICollection<string> Branches { get; set; }

        public string CourseIdentificator { get; set; }
        public string CourseName { get; set; }
        public string CourseDateOfStart { get; set; }
        public string CourseDateOfEnd { get; set; }
        public ICollection<DisplayExamNameTypeViewModel> Exams { get; set; }
    }
}
