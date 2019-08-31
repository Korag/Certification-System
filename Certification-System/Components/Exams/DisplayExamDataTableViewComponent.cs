using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamDataTableViewModel examDataTableViewModel = new DisplayExamDataTableViewModel
            {
                Exams = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamDataTable", examDataTableViewModel);
        }
    }
}
