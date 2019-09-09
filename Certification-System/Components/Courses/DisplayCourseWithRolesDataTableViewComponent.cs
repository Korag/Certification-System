using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseWithRolesDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCourseWithUserRoleViewModel> courseViewModel, string tableIdentificator, int operationSet, string userIdentificator = null)
        {
            DisplayCourseWithUserRolesDataTableViewModel courseDataTableViewModel = new DisplayCourseWithUserRolesDataTableViewModel
            {
                Courses = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseWithRolesDataTable", courseDataTableViewModel);
        }
    }
}
