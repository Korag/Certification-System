using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamResultToUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamResultToUserViewModel> examResultViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamResultToUserDataTableViewModel examResultDataTableViewModel = new DisplayExamResultToUserDataTableViewModel
            {
                ExamsResults = examResultViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamResultToUserDataTable", examResultDataTableViewModel);
        }
    }
}
