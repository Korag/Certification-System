using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenDegreeDetailsViewModel
    {
        [Display(Name = "Nadany stopień zawodowy")]
        public DisplayGivenDegreeToUserExtendedViewModel GivenDegree { get; set; }

        [Display(Name = "Wymagane certyfikaty")]
        public ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> RequiredCertificatesWithGivenInstances { get; set; }

        [Display(Name = "Wymagane stopnie zawodowe")]
        public ICollection<DisplayGivenDegreeToUserViewModel> RequiredDegreesWithGivenInstances { get; set; }

        [Display(Name = "Posiadacz stopnia zawodowego")]
        public DisplayAllUserInformationViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
