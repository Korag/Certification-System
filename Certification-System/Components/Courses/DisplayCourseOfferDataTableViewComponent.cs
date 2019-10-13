using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayCourseOfferDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayCourseOfferViewModel> courseViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayCourseOfferDataTableViewModel courseDataTableViewModel = new DisplayCourseOfferDataTableViewModel
            {
                Courses = courseViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayCourseOfferDataTable", courseDataTableViewModel);
        }
    }
}
