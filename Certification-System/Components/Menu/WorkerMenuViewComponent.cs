using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class WorkerMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_WorkerMenu");
        }
    }
}
