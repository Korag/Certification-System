using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingWithoutCourseDataTableViewModel
    {
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
