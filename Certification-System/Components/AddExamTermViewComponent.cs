using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class AddExamTermViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string orderNumber)
        {
            return View("_AddExamTerm", orderNumber);
        }
    }
}
