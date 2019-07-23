using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class VerifyCertificateViewModel
    {
        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string CertificateIdentificator { get; set; }
    }
}
