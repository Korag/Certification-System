using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamResultDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamResultViewModel> examResultViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamResultDataTableViewModel examResultDataTableViewModel = new DisplayExamResultDataTableViewModel
            {
                ExamsResults = examResultViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamResultDataTable", examResultDataTableViewModel);
        }
    }
}
