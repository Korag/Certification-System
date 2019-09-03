using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenDegreeToUserDataTableViewModel
    {
        public ICollection<DisplayGivenDegreeToUserViewModel> GivenDegrees { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
