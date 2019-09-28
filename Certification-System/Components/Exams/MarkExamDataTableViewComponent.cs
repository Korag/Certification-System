using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class MarkExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MarkUserViewModel[] userViewModel, string tableIdentificator, int operationSet = 0)
        {
            MarkExamDataTableViewModel userDataTableViewModel = new MarkExamDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_MarkExamDataTable", userDataTableViewModel);
        }
    }
}
