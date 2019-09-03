using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenDegreeToUserExtendedDataTableViewModel
    {
        public ICollection<DisplayGivenDegreeToUserExtendedViewModel> GivenDegrees { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
