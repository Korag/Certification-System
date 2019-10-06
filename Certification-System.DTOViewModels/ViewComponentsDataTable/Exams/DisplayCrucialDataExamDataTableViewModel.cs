using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataExamDataTableViewModel
    {
        public ICollection<DisplayCrucialDataExamViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
