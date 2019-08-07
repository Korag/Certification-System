using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class InstructorExaminatorMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_InstructorExaminatorMenu");
        }
    }
}
