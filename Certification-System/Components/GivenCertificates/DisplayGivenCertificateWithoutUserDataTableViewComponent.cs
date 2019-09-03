using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithoutUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithoutUserDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithoutUserDataTableViewModel
            {
                GivenCertificates = givenCertificateViewModel,
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
