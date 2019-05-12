using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class CompanyMenuViewComponent : ViewComponent
    {
        [Authorize(Roles = "Company")]
        public IViewComponentResult Invoke()
        {
            return View("_CompanyMenu");
        }
    }
}
