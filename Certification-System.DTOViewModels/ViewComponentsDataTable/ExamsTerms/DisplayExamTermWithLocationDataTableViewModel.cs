using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermWithLocationDataTableViewModel
    {
        public ICollection<DisplayExamTermWithLocationViewModel> ExamsTerms{ get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
