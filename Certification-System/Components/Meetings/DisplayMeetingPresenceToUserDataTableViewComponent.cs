using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingPresenceToUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayMeetingWithUserPresenceInformation> meetingViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingWithUserPresenceInformationDataTableViewModel meetingDataTableViewModel = new DisplayMeetingWithUserPresenceInformationDataTableViewModel
            {
                Meetings = meetingViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayMeetingPresenceToUserDataTable", meetingDataTableViewModel);
        }
    }
}
