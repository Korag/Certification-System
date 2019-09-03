using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingPresenceDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, ICollection<string> attendanceViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingPresenceDataTableViewModel meetingDataTableViewModel = new DisplayMeetingPresenceDataTableViewModel
            {
                Users = userViewModel,
                AttendanceList = attendanceViewModel,
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
