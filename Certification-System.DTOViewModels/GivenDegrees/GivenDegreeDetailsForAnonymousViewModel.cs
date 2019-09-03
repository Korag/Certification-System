using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenDegreeDetailsForAnonymousViewModel
    {
        [Display(Name = "Nadany stopień zawodowy")]
        public DisplayGivenDegreeToUserExtendedViewModel GivenDegree { get; set; }

        [Display(Name = "Posiadacz stopnia zawodowego")]
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
