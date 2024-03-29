﻿using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class CheckMeetingPresenceDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public PresenceCheckBoxViewModel[] AttendanceList { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
