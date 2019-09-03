using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenDegreeToUserExtDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenDegreeToUserExtendedViewModel> givenDegreeViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenDegreeToUserExtendedDataTableViewModel givenDegreeDataTableViewModel = new DisplayGivenDegreeToUserExtendedDataTableViewModel
            {
                GivenDegrees = givenDegreeViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenDegreeToUserExtDataTable", givenDegreeDataTableViewModel);
        }
    }
}
