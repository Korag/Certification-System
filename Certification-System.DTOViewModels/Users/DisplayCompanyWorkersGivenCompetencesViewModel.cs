using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyWorkersGivenCompetencesViewModel
    {
        public string CompanyIdentificator { get; set; }

        [Display(Name = "Certyfikaty pracowników przedsiębiorstwa")]
        public ICollection<DisplayGivenCertificateViewModel> GivenCertificates { get; set; }

        [Display(Name = "Stopnie zawodowe pracowników przedsiębiorstwa")]
        public ICollection<DisplayGivenDegreeViewModel> GivenDegrees { get; set; }
    }
}
