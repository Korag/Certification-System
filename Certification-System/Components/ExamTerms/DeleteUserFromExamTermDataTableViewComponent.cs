using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DeleteUserFromExamTermDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayCrucialDataUserViewModel> userViewModel, DeleteUsersFromCheckBoxViewModel[] userToDeleteViewModel, string tableIdentificator, int operationSet = 0)
        {
            DeleteUserFromExamTermDataTableViewModel userDataTableViewModel = new DeleteUserFromExamTermDataTableViewModel
            {
                Users = userViewModel,
                UsersToDeleteFromExamTerm = userToDeleteViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DeleteUserFromExamTermDataTable", userDataTableViewModel);
        }
    }
}
