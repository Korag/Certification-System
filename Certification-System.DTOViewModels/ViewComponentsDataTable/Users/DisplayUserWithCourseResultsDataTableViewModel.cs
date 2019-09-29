using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithCourseResultsDataTableViewModel
    {
        public ICollection<DisplayUserWithCourseResultsViewModel> Users { get; set; }
        public ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
