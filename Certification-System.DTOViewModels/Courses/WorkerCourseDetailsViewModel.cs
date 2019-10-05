using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerCourseDetailsViewModel
    {
        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Spotkania w ramach szkolenia")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayExamWithoutCourseViewModel> Exams { get; set; }
    }
}
