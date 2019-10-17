using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseQueueWithDecisionDataTable : ViewComponent
    {
        public IViewComponentResult Invoke(List<CourseQueueNotificationViewModel> courseViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCourseQueueWithDecisionDataTableViewModel courseDataTableViewModel = new DisplayCourseQueueWithDecisionDataTableViewModel
            {
                CoursesNotifications = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseQueueWithDecisionDataTable", courseDataTableViewModel);
        }
    }
}
