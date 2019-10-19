using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCompanyUserPersonalLogDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayLogInformationExtendedViewModel> logViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserPersonalLogDataExtendedTableViewModel userDataTableViewModel = new DisplayUserPersonalLogDataExtendedTableViewModel
            {
                Logs = logViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCompanyUserPersonalLogDataTable", userDataTableViewModel);
        }
    }
}
