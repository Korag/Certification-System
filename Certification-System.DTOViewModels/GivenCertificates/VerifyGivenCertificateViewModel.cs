using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class VerifyGivenCertificateViewModel
    {
        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIdentificator { get; set; }

        public bool GivenCertificateIdentificatorNotExist { get; set; }
        public bool GivenCertificateIdentificatorBadFormat { get; set; }
    }
}
