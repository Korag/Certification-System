using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DeleteUserFromExamTermDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromExamTerm { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
