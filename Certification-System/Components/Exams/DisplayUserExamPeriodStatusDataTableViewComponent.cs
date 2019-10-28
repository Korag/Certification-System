using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserCourseExamPeriodStatusDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            List<DisplayUserWithCourseExamPeriodsResultsViewModel> userViewModel, 
            DispenseGivenCertificateCheckBoxViewModel[] givenCertificateViewModel, 
            ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> examViewModel, 
            bool courseEnded, string tableIdentificator, int operationSet = 0)
        {
            DisplayUserWithCourseExamPeriodsResultsDataTableViewModel userDataTableViewModel = new DisplayUserWithCourseExamPeriodsResultsDataTableViewModel
            {
                Users = userViewModel,
                DispensedGivenCertificates = givenCertificateViewModel,            
                LastExamsPeriods = examViewModel,
                CourseEnded = courseEnded,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayUserCourseExamPeriodStatusDataTable", userDataTableViewModel);
        }
    }
}
