using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithoutCourseDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateWithoutCourseViewModel> givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateWithoutCourseDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateWithoutCourseDataTableViewModel
            {
                GivenCertificates = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateWithoutCourseDataTable", givenCertificateDataTableViewModel);
        }
    }
}
