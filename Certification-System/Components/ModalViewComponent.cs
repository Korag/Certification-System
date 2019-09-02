using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Components
{
    public class ModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title, string body, string action, string controller, bool formSubmit, string buttonText, string buttonClass, string dataTarget = "modal")
        {
            ModalViewModel modal = new ModalViewModel
            {
                Title = title,
                Body = body,

                Action = action,
                Controller = controller,

                FormSubmit = formSubmit,
                DataTarget = dataTarget,

                ButtonText = buttonText,
                ButtonClass = buttonClass
            };

            return View("_Modal", modal);
        }
    }
}
