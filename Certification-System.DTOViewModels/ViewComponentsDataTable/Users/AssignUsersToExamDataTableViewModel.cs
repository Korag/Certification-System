using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class AssignUsersToExamDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public AddUsersFromCheckBoxViewModel[] UsersToAssignToExam { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
