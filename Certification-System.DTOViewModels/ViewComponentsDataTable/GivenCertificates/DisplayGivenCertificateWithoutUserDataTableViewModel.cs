using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateWithoutUserDataTableViewModel
    {
        public ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
