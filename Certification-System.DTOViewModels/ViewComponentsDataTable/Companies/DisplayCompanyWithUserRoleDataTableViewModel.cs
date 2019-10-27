using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyWithUserRoleDataTableViewModel
    {
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
