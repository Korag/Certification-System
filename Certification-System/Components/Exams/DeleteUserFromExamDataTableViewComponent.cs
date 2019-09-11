using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DeleteUserFromExamDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayCrucialDataUserViewModel> userViewModel, DeleteUsersFromCheckBoxViewModel[] userToDeleteViewModel, string tableIdentificator, int operationSet = 0)
        {
            DeleteUserFromExamDataTableViewModel userDataTableViewModel = new DeleteUserFromExamDataTableViewModel
            {
                Users = userViewModel,
                UsersToDeleteFromExam = userToDeleteViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DeleteUserFromExamDataTable", userDataTableViewModel);
        }
    }
}
