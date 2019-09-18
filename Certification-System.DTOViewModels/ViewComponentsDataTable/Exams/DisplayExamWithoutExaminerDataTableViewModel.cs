using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithoutExaminerDataTableViewModel
    {
        public ICollection<DisplayExamWithoutExaminerViewModel> Exams { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
