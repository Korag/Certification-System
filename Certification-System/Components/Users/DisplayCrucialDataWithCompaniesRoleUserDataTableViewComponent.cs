using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataWithCompaniesRoleUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataWithCompaniesRoleUserViewModel> userViewModel, DisplayCourseViewModel courseViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCrucialDataWithCompaniesRoleUserDataTableViewModel userDataTableViewModel = new DisplayCrucialDataWithCompaniesRoleUserDataTableViewModel
            {
                Users = userViewModel,
                Course = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataWithCompaniesUserDataTable", userDataTableViewModel);
        }
    }
}
