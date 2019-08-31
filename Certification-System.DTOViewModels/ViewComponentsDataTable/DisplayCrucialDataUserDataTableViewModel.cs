using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataUserDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
