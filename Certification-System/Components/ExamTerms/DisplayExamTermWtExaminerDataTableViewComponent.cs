using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamTermWtExaminerDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamTermWithoutExaminerViewModel> examTermViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamTermWithoutExaminerDataTableViewModel examTermDataTableViewModel = new DisplayExamTermWithoutExaminerDataTableViewModel
            {
                ExamsTerms = examTermViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamTermWtExaminerDataTable", examTermDataTableViewModel);
        }
    }
}
