using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataExamViewModel> examViewModel, bool courseEnded, string tableIdentificator, int operationSet = 0)
        {
            DisplayCrucialDataExamDataTableViewModel examDataTableViewModel = new DisplayCrucialDataExamDataTableViewModel
            {
                Exams = examViewModel,
                CourseEnded = courseEnded,
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
