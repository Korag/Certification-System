using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataExamExtDataTableViewModel
    {
        public ICollection<DisplayCrucialDataExamViewModel> Exams { get; set; }
        public bool CourseEnded { get; set; }
        public ICollection<string> LastingExamsIndexers { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
