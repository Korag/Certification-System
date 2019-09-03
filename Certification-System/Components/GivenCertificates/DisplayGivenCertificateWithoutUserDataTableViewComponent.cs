using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithoutUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DisplayGivenCertificateToUserWithoutCourseExtendedViewModel givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithoutUserExtendedDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithoutUserExtendedDataTableViewModel
            {
                GivenCertificate = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateWithoutUserDataTable", givenCertificateDataTableViewModel);
        }
    }
}
