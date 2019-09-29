using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermWithoutExaminerDataTableViewModel
    {
        public ICollection<DisplayExamTermWithoutExaminerViewModel> ExamsTerms{ get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
