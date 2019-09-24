using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingPresenceDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        [Display(Name="Obecność")]
        public ICollection<string> AttendanceList { get; set; }
        public DateTime MeetingStartDate { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
