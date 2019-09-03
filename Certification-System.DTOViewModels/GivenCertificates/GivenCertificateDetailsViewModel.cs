using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenCertificateDetailsViewModel
    {
        [Display(Name = "Nadany certyfikat")]
        public DisplayGivenCertificateToUserWithoutCourseExtendedViewModel GivenCertificate { get; set; }

        [Display(Name = "Dane właściciela certyfikatu")]
        public DisplayAllUserInformationViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Instruktorzy")]
        public ICollection<DisplayCrucialDataWithContactUserViewModel> Instructors { get; set; }

        [Display(Name = "Spotkania w ramach kursu")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }
    }
}
