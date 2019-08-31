using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataWithContactUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayCrucialDataWithContactUserViewModel> userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCrucialDataWithContactDataTableViewModel userDataTableViewModel = new DisplayCrucialDataWithContactDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataWithContactDataTable", userDataTableViewModel);
        }
    }
}
