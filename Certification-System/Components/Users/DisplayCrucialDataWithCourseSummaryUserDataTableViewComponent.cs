using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCrucialDataWithCourseSummaryUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCrucialDataUserViewModel> userViewModel, DisplayCourseViewModel courseViewModel, DispenseGivenCertificateCheckBoxViewModel[] dispensedGivenCertificateViewModel ,string tableIdentificator, int operationSet = 0)
        {
            DisplayCrucialDataWithCourseSummaryUserDataTableViewModel userDataTableViewModel = new DisplayCrucialDataWithCourseSummaryUserDataTableViewModel
            {
                Users = userViewModel,
                DispensedGivenCertificates = dispensedGivenCertificateViewModel,
                Course = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCrucialDataCourseSummaryUserDataTable", userDataTableViewModel);
        }
    }
}
