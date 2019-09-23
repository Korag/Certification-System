using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamTermWtExamWhLocationDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamTermWithoutExamWithLocationViewModel> examTermViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamTermWithoutExamWithLocationDataTableViewModel examTermDataTableViewModel = new DisplayExamTermWithoutExamWithLocationDataTableViewModel
            {
                ExamsTerms = examTermViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamTermWtExamWhLocationDataTable", examTermDataTableViewModel);
        }
    }
}
