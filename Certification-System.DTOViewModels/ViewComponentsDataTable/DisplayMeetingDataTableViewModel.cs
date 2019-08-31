using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingDataTableViewModel
    {
        public ICollection<DisplayMeetingViewModel> Meetings { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
