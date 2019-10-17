using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseQueueWithDecisionDataTableViewModel
    {
        public ICollection<CourseQueueNotificationViewModel> CoursesNotifications { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
