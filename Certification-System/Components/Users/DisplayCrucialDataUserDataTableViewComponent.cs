using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, string tableIdentificator, string courseIdentificator = null, int operationSet = 0)
        {
            DisplayCrucialDataUserDataTableViewModel userDataTableViewModel = new DisplayCrucialDataUserDataTableViewModel
            {
                Users = userViewModel,
                CourseIdentificator = courseIdentificator,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataUserDataTable", userDataTableViewModel);
        }
    }
}
