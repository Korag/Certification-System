using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateWithoutUserExtendedDataTableViewModel
    {
        public ICollection<DisplayGivenCertificateToUserWithoutCourseExtendedViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
