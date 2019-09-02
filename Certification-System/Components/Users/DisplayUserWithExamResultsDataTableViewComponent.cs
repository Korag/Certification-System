using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserWithExamResultsDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayUserWithExamResults> userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserWithExamResultsDataTableViewModel userDataTableViewModel = new DisplayUserWithExamResultsDataTableViewModel
            {
                Users = userViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayUserWithExamResultsDataTable", userDataTableViewModel);
        }
    }
}
