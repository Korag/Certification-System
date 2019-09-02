using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayAllUserInformationDataTableViewModel
    {
        public ICollection<DisplayAllUserInformationViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
