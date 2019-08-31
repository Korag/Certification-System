using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenDegreeDataTableViewModel
    {
        public ICollection<DisplayGivenDegreeViewModel> GivenDegrees { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
