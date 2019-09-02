using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamDataTableViewModel
    {
        public ICollection<DisplayExamViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
