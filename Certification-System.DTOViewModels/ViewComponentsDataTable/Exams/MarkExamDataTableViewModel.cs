using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class MarkExamDataTableViewModel
    {
        public ICollection<MarkUserViewModel> Users { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
