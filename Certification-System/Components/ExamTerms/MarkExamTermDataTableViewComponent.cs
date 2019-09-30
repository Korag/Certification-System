using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class MarkExamTermDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MarkUserViewModel[] userViewModel, string tableIdentificator, int operationSet = 0)
        {
            MarkExamTermDataTableViewModel examTermDataTableViewModel = new MarkExamTermDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_MarkExamTermDataTable", examTermDataTableViewModel);
        }
    }
}
