using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class VerifyCertificateViewModel
    {
        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIdentificator { get; set; }

        public bool CertificateIdentificatorNotExist { get; set; }
        public bool CertificateIdentificatorBadFormat { get; set; }
    }
}
