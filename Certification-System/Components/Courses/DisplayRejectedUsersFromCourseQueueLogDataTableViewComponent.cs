using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayRejectedUsersFromCourseQueueLogDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayRejectedUserLog> logViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayRejectedUsersFromCourseQueueLogDataTableViewModel userDataTableViewModel = new DisplayRejectedUsersFromCourseQueueLogDataTableViewModel
            {
                Logs = logViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayRejectedUsersFromCourseQueueLogDataTable", userDataTableViewModel);
        }
    }
}
