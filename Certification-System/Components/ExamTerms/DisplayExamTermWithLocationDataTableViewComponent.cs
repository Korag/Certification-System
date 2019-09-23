using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamTermWithLocationDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamTermWithLocationViewModel> examTermViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamTermWithLocationDataTableViewModel examTermDataTableViewModel = new DisplayExamTermWithLocationDataTableViewModel
            {
                ExamsTerms = examTermViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamTermWithLocationDataTable", examTermDataTableViewModel);
        }
    }
}
