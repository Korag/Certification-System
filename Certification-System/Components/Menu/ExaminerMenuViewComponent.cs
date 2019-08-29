using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class ExaminerMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_ExaminerMenu");
        }
    }
}
