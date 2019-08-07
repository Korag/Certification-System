using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class InstructorMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_InstructorMenu");
        }
    }
}
