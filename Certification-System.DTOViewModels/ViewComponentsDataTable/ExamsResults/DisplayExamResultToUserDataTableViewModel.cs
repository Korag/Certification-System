using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamResultToUserDataTableViewModel
    {
        public ICollection<DisplayExamResultToUserViewModel> ExamsResults { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
