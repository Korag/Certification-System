using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DispenseGivenCertificatesDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayUserWithCourseResultsViewModel> userViewModel, DispenseGivenCertificateCheckBoxViewModel[] givenCertificateViewModel, ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DispenseGivenCertificatesDataTableViewModel userDataTableViewModel = new DispenseGivenCertificatesDataTableViewModel
            {
                Users = userViewModel,
                DispensedGivenCertificates = givenCertificateViewModel,
                LastExamsPeriods = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DispenseGivenCertificatesDataTable", userDataTableViewModel);
        }
    }
}
