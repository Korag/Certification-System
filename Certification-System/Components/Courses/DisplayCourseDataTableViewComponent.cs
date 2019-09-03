using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCourseViewModel> courseViewModel, string tableIdentificator, int operationSet, string userIdentificator = null)
        {
            DisplayCourseDataTableViewModel courseDataTableViewModel = new DisplayCourseDataTableViewModel
            {
                Courses = courseViewModel,
                UserIdentificator = userIdentificator,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseDataTable", courseDataTableViewModel);
        }
    }
}
