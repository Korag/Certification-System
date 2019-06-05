using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class VerifyCertificateViewModel
    {
        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string CertificateIdentificator { get; set; }
    }
}
