using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCertificateDataTableViewModel
    {
        public ICollection<DisplayCertificateViewModel> Certificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
