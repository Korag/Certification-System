using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingWithoutInstructorDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayMeetingWithoutInstructorViewModel> meetingViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingWithoutInstructorDataTableViewModel meetingDataTableViewModel = new DisplayMeetingWithoutInstructorDataTableViewModel
            {
                Meetings = meetingViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayMeetingWithoutInstructorDataTable", meetingDataTableViewModel);
        }
    }
}
