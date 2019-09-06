using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class AssignUsersToExamTermDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, AddUsersFromCheckBoxViewModel[] userToAddViewModel, string tableIdentificator, int operationSet = 0)
        {
            AssignUsersToExamTermDataTableViewModel userDataTableViewModel = new AssignUsersToExamTermDataTableViewModel
            {
                Users = userViewModel,
                UsersToAssignToExamTerm = userToAddViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_AssignUsersToExamTermDataTable", userDataTableViewModel);
        }
    }
}
