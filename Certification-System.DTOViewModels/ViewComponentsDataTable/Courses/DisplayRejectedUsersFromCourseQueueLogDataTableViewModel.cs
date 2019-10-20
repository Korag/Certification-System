using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayRejectedUsersFromCourseQueueLogDataTableViewModel
    {
        public ICollection<DisplayRejectedUserLog> Logs { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
