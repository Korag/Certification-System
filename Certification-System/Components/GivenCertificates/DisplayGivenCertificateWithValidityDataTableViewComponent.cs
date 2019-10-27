using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayGivenCertificateWithValidityDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayGivenCertificateViewModel> givenCertificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayGivenCertificateDataTableViewModel givenCertificateDataTableViewModel = new DisplayGivenCertificateDataTableViewModel
            {
                GivenCertificates = givenCertificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayGivenCertificateWithValidityDataTable", givenCertificateDataTableViewModel);
        }
    }
}
