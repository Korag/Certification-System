using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingPresenceDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public ICollection<string> AttendanceList { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
