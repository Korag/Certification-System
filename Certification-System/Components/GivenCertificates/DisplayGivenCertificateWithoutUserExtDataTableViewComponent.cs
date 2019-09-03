using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithoutUserExtDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateToUserWithoutCourseExtendedViewModel> givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithoutUserExtendedDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithoutUserExtendedDataTableViewModel
            {
                GivenCertificates = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateW-outUserDataTable", givenCertificateDataTableViewModel);
        }
    }
}
