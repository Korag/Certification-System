using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class CompanyMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_CompanyMenu");
        }
    }
}
