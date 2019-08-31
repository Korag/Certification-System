using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamResultDataTableViewModel
    {
        public ICollection<DisplayExamResultViewModel> ExamsResults { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
