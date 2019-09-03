using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenCertificateDetailsForAnonymousViewModel
    {
        [Display(Name = "Nadany certyfikat")]
        public DisplayGivenCertificateToUserWithoutCourseViewModel GivenCertificate { get; set; }

        [Display(Name = "Dane właściciela certyfikatu")]
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
