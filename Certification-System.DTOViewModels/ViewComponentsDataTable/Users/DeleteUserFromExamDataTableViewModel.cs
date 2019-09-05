using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DeleteUserFromExamDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromExam { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
