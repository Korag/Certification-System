using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class AdminMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_AdminMenu");
        }
    }
}
