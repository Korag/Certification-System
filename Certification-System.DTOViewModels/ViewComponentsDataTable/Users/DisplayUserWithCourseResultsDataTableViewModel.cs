using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithCourseResultsDataTableViewModel
    {
        public ICollection<DisplayUserWithCourseResultsViewModel> Users { get; set; }
        public DispenseGivenCertificateCheckBoxViewModel[] DispensedGivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
