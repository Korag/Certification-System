using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayExamWithoutExaminerDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayExamWithoutExaminerViewModel> examViewModel, string tableIdentificator, int operationSet = 0)
        {
            DisplayExamWithoutExaminerDataTableViewModel examDataTableViewModel = new DisplayExamWithoutExaminerDataTableViewModel
            {
                Exams = examViewModel,
                Options = new DataTableOptionsViewModel
                {
                    TableIdentificator = tableIdentificator,
                    OperationSet = operationSet
                }
            };

            return View("_DisplayExamWithoutExaminerDataTable", examDataTableViewModel);
        }
    }
}
