using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseOfferDataTableViewModel
    {
        public ICollection<DisplayCourseOfferViewModel> Courses { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
