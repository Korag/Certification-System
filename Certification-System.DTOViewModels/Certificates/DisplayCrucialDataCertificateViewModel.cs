using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataCertificateViewModel
    {
        public string CertificateIdentificator{ get; set; }

        [Display(Name = "Identyfikator")]
        public string CertificateIndexer { get; set; }

        [Display(Name = "Nazwa certyfikatu")]
        public string Name { get; set; }
    }
}
