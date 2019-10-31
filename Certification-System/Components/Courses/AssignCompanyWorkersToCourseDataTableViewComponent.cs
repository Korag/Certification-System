using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class AssignCompanyWorkersToCourseDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, AddUsersFromCheckBoxViewModel[] userToAddViewModel, string tableIdentificator, int operationSet = 0)
        {
            AssignCompanyWorkersToCourseDataTableViewModel userDataTableViewModel = new AssignCompanyWorkersToCourseDataTableViewModel
            {
                Users = userViewModel,
                CompanyWorkersToAssignToCourse = userToAddViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_AssignCompanyWorkersToCourseDataTable", userDataTableViewModel);
        }
    }
}
