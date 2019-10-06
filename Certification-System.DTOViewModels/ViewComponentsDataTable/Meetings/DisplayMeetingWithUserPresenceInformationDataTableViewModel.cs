using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingWithUserPresenceInformationDataTableViewModel
    {
        public ICollection<DisplayMeetingWithUserPresenceInformation> Meetings { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
