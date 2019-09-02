using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithExamResultsDataTableViewModel
    {
        public ICollection<DisplayUserWithExamResults> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
