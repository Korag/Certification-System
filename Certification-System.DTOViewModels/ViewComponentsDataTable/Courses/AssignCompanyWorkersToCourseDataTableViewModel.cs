using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class AssignCompanyWorkersToCourseDataTableViewModel
    {
        public ICollection<DisplayCrucialDataUserViewModel> Users { get; set; }
        public AddUsersFromCheckBoxViewModel[] CompanyWorkersToAssignToCourse { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
