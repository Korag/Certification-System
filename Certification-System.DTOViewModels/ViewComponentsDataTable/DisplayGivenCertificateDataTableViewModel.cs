using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateDataTableViewModel
    {
        public ICollection<DisplayGivenCertificateViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
