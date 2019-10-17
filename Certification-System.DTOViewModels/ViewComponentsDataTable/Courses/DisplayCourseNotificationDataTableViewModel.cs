using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseNotificationDataTableViewModel
    {
        public ICollection<DisplayCourseNotificationViewModel> Courses { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
