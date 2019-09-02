using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermDataTableViewModel
    {
        public ICollection<DisplayExamTermViewModel> ExamsTerms{ get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
