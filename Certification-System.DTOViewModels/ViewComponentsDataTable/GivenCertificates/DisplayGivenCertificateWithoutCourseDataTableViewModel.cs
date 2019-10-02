using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateWithoutCourseDataTableViewModel
    {
        public ICollection<DisplayGivenCertificateWithoutCourseViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
