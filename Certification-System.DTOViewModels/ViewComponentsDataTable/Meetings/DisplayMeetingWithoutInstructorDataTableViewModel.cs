﻿using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingWithoutInstructorDataTableViewModel
    {
        public ICollection<DisplayMeetingWithoutInstructorViewModel> Meetings { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
