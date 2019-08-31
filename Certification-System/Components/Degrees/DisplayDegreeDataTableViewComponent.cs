using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayDegreeDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayDegreeViewModel> degreeViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayDegreeDataTableViewModel degreeDataTableViewModel = new DisplayDegreeDataTableViewModel
            {
                Degrees = degreeViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayDegreeDataTable", degreeDataTableViewModel);
        }
    }
}
