using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class WorkerMenuViewComponent : ViewComponent
    {
        [Authorize(Roles="Worker")]
        public IViewComponentResult Invoke()
        {
            return View("_WorkerMenu");
        }
    }
}
