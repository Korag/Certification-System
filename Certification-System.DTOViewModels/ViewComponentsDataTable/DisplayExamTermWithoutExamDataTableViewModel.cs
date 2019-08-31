using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermWithoutExamDataTableViewModel
    {
        public ICollection<DisplayExamTermWithoutExamViewModel> ExamsTerms{ get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
