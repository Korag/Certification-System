using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayUserViewModel> userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserDataTableViewModel userDataTableViewModel = new DisplayUserDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayUserDataTable", userDataTableViewModel);
        }
    }
}
