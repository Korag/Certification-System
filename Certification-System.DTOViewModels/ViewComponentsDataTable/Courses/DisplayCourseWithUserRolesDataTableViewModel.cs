using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseWithUserRolesDataTableViewModel
    {
        public ICollection<DisplayCourseWithUserRoleViewModel> Courses { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
