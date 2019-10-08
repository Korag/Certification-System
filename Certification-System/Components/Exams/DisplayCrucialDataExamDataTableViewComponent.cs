using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataExamViewModel> examViewModel, bool courseEnded, string tableIdentificator, ICollection<string> lastingExamViewModel = null, int operationSet = 0)
        {
            if (lastingExamViewModel == null)
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

            DisplayCrucialDataExamExtDataTableViewModel examDataTableExtendedViewModel = new DisplayCrucialDataExamExtDataTableViewModel
            {
                Exams = examViewModel,
                CourseEnded = courseEnded,
                LastingExamsIndexers = lastingExamViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataExamExtDataTable", examDataTableExtendedViewModel);
        }
    }
}
