using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataWithCourseSummaryUserDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public DispenseGivenCertificateCheckBoxViewModel[] DispensedGivenCertificates { get; set; }
        public DisplayCourseWithPriceViewModel Course { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
