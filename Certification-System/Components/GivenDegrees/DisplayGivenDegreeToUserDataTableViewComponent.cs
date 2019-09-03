using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenDegreeToUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenDegreeToUserViewModel> givenDegreeViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenDegreeToUserDataTableViewModel givenDegreeDataTableViewModel = new DisplayGivenDegreeToUserDataTableViewModel
            {
                GivenDegrees = givenDegreeViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenDegreeToUserDataTable", givenDegreeDataTableViewModel);
        }
    }
}
