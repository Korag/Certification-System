using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserPersonalLogDataExtendedTableViewModel
    {
        public ICollection<DisplayLogInformationExtendedViewModel> Logs { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}