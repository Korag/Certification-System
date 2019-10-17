using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class AdminNotificationViewModel
    {
        public ICollection<CourseQueueNotificationViewModel> CourseQueueNotification { get; set; }
        public ICollection<DisplayCourseNotificationViewModel> NotEndedCoursesAfterEndDate { get; set; }
    }
}