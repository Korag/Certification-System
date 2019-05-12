using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class AdminMenuViewComponent : ViewComponent
    {
        [Authorize(Roles = "Admin")]
        public IViewComponentResult Invoke()
        {
            return View("_AdminMenu");
        }
    }
}
