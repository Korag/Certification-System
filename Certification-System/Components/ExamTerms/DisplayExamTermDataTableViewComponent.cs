using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamTermDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamTermViewModel> examTermViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamTermDataTableViewModel examTermDataTableViewModel = new DisplayExamTermDataTableViewModel
            {
                ExamsTerms = examTermViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamTermDataTable", examTermDataTableViewModel);
        }
    }
}
