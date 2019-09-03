using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DisplayGivenCertificateToUserWithoutCourseViewModel givenCertificateViewModel, DisplayCrucialDataWithBirthDateUserViewModel userViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithUserDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithUserDataTableViewModel
            {
                User = userViewModel,
                GivenCertificate = givenCertificateViewModel,
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
