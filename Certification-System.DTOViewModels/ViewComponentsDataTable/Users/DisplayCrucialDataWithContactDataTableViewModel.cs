using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataWithContactDataTableViewModel
    {
        public ICollection<DisplayCrucialDataWithContactUserViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
