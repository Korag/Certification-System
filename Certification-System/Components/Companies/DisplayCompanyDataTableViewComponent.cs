using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCompanyDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCompanyViewModel> companyViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCompanyDataTableViewModel companyDataTableViewModel = new DisplayCompanyDataTableViewModel
            {
                Companies = companyViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCompanyDataTable", companyDataTableViewModel);
        }
    }
}
