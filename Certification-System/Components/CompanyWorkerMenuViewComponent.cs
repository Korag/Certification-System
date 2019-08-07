using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class CompanyWorkerMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_CompanyWorkerMenu");
        }
    }
}
