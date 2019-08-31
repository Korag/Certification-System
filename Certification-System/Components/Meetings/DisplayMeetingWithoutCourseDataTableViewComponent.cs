using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayMeetingWithoutCourseDataTableViewComponen : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayMeetingWithoutCourseViewModel> meetingViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayMeetingWithoutCourseDataTableViewModel meetingDataTableViewModel = new DisplayMeetingWithoutCourseDataTableViewModel
            {
                Meetings = meetingViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayMeetingWithoutCourseDataTable", meetingDataTableViewModel);
        }
    }
}
