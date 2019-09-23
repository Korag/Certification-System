using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermWithoutExamWithLocationDataTableViewModel
    {
        public ICollection<DisplayExamTermWithoutExamWithLocationViewModel> ExamsTerms{ get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
