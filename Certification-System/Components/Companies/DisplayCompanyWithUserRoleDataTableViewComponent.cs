using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCompanyWithUserRoleDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCompanyViewModel> companyViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCompanyWithUserRoleDataTableViewModel companyDataTableViewModel = new DisplayCompanyWithUserRoleDataTableViewModel
            {
                Companies = companyViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCompanyWithUserRoleDataTable", companyDataTableViewModel);
        }
    }
}
