using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyWithUserRoleDataTableViewModel
    {
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
        public DisplayAllUserInformationViewModel User { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
