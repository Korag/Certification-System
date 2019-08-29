using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataWithCompaniesRoleUserDataTableViewModel
    {
        public ICollection<DisplayCrucialDataWithCompaniesRoleUserViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
