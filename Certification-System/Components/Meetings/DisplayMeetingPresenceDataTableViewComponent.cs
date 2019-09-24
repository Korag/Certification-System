using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingPresenceDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, ICollection<string> attendanceViewModel, DateTime meetingStartDate, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingPresenceDataTableViewModel meetingDataTableViewModel = new DisplayMeetingPresenceDataTableViewModel
            {
                Users = userViewModel,
                AttendanceList = attendanceViewModel,
                MeetingStartDate = meetingStartDate,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayMeetingPresenceDataTable", meetingDataTableViewModel);
        }
    }
}
