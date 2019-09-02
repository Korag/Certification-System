using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayDegreeDataTableViewModel
    {
        public ICollection<DisplayDegreeViewModel> Degrees { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
