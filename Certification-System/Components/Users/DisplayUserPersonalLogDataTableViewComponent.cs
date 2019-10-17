using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserPersonalLogDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayLogInformationViewModel> logViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserPersonalLogDataTableViewModel userDataTableViewModel = new DisplayUserPersonalLogDataTableViewModel
            {
                Logs = logViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayUserPersonalLogDataTableViewModel", userDataTableViewModel);
        }
    }
}
