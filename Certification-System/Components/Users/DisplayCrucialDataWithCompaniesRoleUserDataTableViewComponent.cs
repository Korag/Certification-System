using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataWithCompaniesRoleUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataWithCompaniesRoleUserViewModel> usersViewModel, string tableIdentificator, int operationSet)
        {
            DisplayCrucialDataWithCompaniesRoleUserDataTableViewModel userDataTableViewModel = new DisplayCrucialDataWithCompaniesRoleUserDataTableViewModel
            {
                Users = usersViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    SelectedSetOfOperations = operationSet
                }
            };

            ViewBag.tableIdentificator = tableIdentificator;

            return View("_DisplayCrucialDataWithCompaniesUserDataTable", userDataTableViewModel);
        }
    }
}
