using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerGivenDegreeDetailsViewModel
    {
        [Display(Name = "Nadany stopień zawodowy")]
        public DisplayGivenDegreeToUserExtendedViewModel GivenDegree { get; set; }

        [Display(Name = "Wymagane certyfikaty")]
        public ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> RequiredCertificatesWithGivenInstances { get; set; }

        [Display(Name = "Wymagane stopnie zawodowe")]
        public ICollection<DisplayGivenDegreeToUserViewModel> RequiredDegreesWithGivenInstances { get; set; }
    }
}
