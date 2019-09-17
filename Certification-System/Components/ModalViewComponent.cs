using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class ModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title, string body, string action, string controller, bool formSubmit, string buttonText, string buttonClass, Dictionary<string, string> arguments, string dataTarget = "modal")
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
                ButtonClass = buttonClass,

                Arguments = arguments
            };

            return View("_Modal", modal);
        }
    }
}
