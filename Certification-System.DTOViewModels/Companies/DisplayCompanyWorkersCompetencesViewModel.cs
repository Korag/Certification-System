using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyWorkersCompetencesViewModel
    {

        [Display(Name = "Certyfikaty pracowników przedsiębiorstwa")]
        public ICollection<DisplayCertificateViewModel> Certificates { get; set; }

        [Display(Name = "Stopnie zawodowe pracowników przedsiębiorstwa")]
        public ICollection<DisplayDegreeViewModel> Degrees { get; set; }
    }
}
