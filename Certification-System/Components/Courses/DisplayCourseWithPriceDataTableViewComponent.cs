using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseWithPriceDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCourseWithPriceViewModel> courseViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCourseWithPriceDataTableViewModel courseDataTableViewModel = new DisplayCourseWithPriceDataTableViewModel
            {
                Courses = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseWithPriceDataTable", courseDataTableViewModel);
        }
    }
}
