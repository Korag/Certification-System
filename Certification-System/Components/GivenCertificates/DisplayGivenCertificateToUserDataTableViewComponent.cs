using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateToUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateToUserViewModel> givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateToUserDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateToUserDataTableViewModel
            {
                GivenCertificates = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateToUserDataTable", givenCertificateDataTableViewModel);
        }
    }
}
