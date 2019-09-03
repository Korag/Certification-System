using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> givenCertificateViewModel, DisplayCrucialDataWithBirthDateUserViewModel userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithUserDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithUserDataTableViewModel
            {
                User = userViewModel,
                GivenCertificates = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateWithUserDataTable", givenCertificateDataTableViewModel);
        }
    }
}
