using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseNotificationDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCourseNotificationViewModel> courseViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCourseNotificationDataTableViewModel courseDataTableViewModel = new DisplayCourseNotificationDataTableViewModel 
            {
                Courses = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseNotificationDataTable", courseDataTableViewModel);
        }
    }
}
