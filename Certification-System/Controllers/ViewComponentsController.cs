using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Controllers
{
    public class ViewComponentsController : Controller
    {
        // GET: GetAddExamTermViewComponent
        [Authorize(Roles = "Admin")]
        public ActionResult GetAddExamTermViewComponent(string orderNumber)
        {
            return ViewComponent("AddExamTerm", new { orderNumber = orderNumber });
        }
    }
}