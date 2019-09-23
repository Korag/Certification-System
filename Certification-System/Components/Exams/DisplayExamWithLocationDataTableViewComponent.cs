using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamWithLocationDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamWithLocationViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamWithLocationDataTableViewModel examDataTableViewModel = new DisplayExamWithLocationDataTableViewModel
            {
                Exams = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamWithLocationDataTable", examDataTableViewModel);
        }
    }
}
