using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DeleteUserFromCourseDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromCourse { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
