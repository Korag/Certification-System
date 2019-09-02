using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayAllUserInformationDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayAllUserInformationViewModel> userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayAllUserInformationDataTableViewModel userDataTableViewModel = new DisplayAllUserInformationDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayAllUserInformationDataTable", userDataTableViewModel);
        }
    }
}
