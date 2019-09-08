using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserWithCourseResultsDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayUserWithCourseResultsViewModel> userViewModel, int examsQuantity, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserWithCourseResultsDataTableViewModel userDataTableViewModel = new DisplayUserWithCourseResultsDataTableViewModel
            {
                Users = userViewModel,
                ExamsQuantity = examsQuantity,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayUserWithCourseResultsDataTable", userDataTableViewModel);
        }
    }
}
