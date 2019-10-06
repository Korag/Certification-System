using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataExamViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCrucialDataExamDataTableViewModel examDataTableViewModel = new DisplayCrucialDataExamDataTableViewModel
            {
                Exams = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataExamDataTable", examDataTableViewModel);
        }
    }
}
