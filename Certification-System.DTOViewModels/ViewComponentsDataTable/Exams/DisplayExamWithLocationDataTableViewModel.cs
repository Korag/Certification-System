using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithLocationDataTableViewModel
    {
        public ICollection<DisplayExamWithLocationViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
