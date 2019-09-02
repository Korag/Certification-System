using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyDataTableViewModel
    {
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
