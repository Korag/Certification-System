using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCertificateDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCertificateViewModel> certificateViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCertificateDataTableViewModel certificateDataTableViewModel = new DisplayCertificateDataTableViewModel
            {
                Certificates = certificateViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCertificateDataTable", certificateDataTableViewModel);
        }
    }
}
