using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseWithPriceDataTableViewModel
    {
        public ICollection<DisplayCourseWithPriceViewModel> Courses { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
