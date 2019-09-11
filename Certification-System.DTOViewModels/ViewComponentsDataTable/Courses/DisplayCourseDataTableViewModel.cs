using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseDataTableViewModel
    {
        public ICollection<DisplayCourseViewModel> Courses { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
