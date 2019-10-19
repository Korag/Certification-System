using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserPersonalLogDataTableViewModel
    {
        public ICollection<DisplayLogInformationViewModel> Logs { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}