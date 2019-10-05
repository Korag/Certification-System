using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerGivenCertificateDetailsViewModel
    {
        [Display(Name = "Nadany certyfikat")]
        public DisplayGivenCertificateToUserWithoutCourseExtendedViewModel GivenCertificate { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Spotkania w ramach kursu")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }
    }
}
