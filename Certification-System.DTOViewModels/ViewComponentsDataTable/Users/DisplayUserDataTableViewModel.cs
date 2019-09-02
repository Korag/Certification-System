using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserDataTableViewModel
    {
        public ICollection<DisplayUserViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
