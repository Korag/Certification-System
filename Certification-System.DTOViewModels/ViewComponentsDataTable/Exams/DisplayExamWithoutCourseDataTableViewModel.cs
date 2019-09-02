using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithoutCourseDataTableViewModel
    {
        public ICollection<DisplayExamWithoutCourseViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
