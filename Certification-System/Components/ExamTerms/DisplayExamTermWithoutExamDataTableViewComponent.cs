using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamTermWithoutExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamTermWithoutExamViewModel> examTermViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamTermWithoutExamDataTableViewModel examTermDataTableViewModel = new DisplayExamTermWithoutExamDataTableViewModel
            {
                ExamsTerms = examTermViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamTermWithoutExamDataTable", examTermDataTableViewModel);
        }
    }
}
