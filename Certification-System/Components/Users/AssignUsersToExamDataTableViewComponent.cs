using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class AssignUsersToExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, AddUsersFromCheckBoxViewModel[] userToAddViewModel, string tableIdentificator, int operationSet = 0)
        {
            AssignUsersToExamDataTableViewModel userDataTableViewModel = new AssignUsersToExamDataTableViewModel
            {
                Users = userViewModel,
                UsersToAssignToExam = userToAddViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_AssignUsersToExamDataTable", userDataTableViewModel);
        }
    }
}
