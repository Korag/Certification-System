using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenDegreeDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenDegreeViewModel> givenDegreeViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenDegreeDataTableViewModel givenDegreeDataTableViewModel = new DisplayGivenDegreeDataTableViewModel
            {
                GivenDegrees = givenDegreeViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenDegreeDataTable", givenDegreeDataTableViewModel);
        }
    }
}
