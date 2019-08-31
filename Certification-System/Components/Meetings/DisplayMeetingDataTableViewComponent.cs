using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayMeetingViewModel> meetingViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingDataTableViewModel meetingDataTableViewModel = new DisplayMeetingDataTableViewModel
            {
                Meetings = meetingViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayMeetingDataTable", meetingDataTableViewModel);
        }
    }
}
