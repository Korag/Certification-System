using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class CheckMeetingPresenceDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, PresenceCheckBoxViewModel[] presenceViewModel, string tableIdentificator, int operationSet = 0)
        {
            CheckMeetingPresenceDataTableViewModel userDataTableViewModel = new CheckMeetingPresenceDataTableViewModel
            {
                Users = userViewModel,
                AttendanceList = presenceViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_CheckMeetingPresenceDataTable", userDataTableViewModel);
        }
    }
}
