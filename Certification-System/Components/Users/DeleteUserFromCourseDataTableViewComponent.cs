using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DeleteUserFromCourseDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayCrucialDataUserViewModel> userViewModel, DeleteUsersFromCheckBoxViewModel[] userToDeleteViewModel, string tableIdentificator, int operationSet = 0)
        {
            DeleteUserFromCourseDataTableViewModel userDataTableViewModel = new DeleteUserFromCourseDataTableViewModel
            {
                Users = userViewModel,
                UsersToDeleteFromCourse = userToDeleteViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DeleteUserFromCourseDataTable", userDataTableViewModel);
        }
    }
}
