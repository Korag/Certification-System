using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateToUserDataTableViewModel
    {
        public ICollection<DisplayGivenCertificateToUserViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
