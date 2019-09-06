using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class AssignUsersToExamTermDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public AddUsersFromCheckBoxViewModel[] UsersToAssignToExamTerm { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
