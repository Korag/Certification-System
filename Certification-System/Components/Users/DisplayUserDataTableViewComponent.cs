using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DisplayUserViewModel> usersViewModel, string tableIdentificator)
        {
            ViewBag.tableIdentificator = tableIdentificator;

            return View("_DisplayUserDataTable", usersViewModel);
        }
    }
}
