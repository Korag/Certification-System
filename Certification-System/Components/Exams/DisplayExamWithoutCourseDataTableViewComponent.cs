using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamWithoutCourseDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamWithoutCourseViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamWithoutCourseDataTableViewModel examDataTableViewModel = new DisplayExamWithoutCourseDataTableViewModel
            {
                Exams = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamWithoutCourseDataTable", examDataTableViewModel);
        }
    }
}
