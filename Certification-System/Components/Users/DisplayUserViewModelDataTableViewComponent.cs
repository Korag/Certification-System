using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Certification_System.Components
{
    public class DisplayUserViewModelDataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ICollection<DisplayUserViewModel> usersViewModel, string tableIdentificator)
        {
            ViewBag.tableIdentificator = tableIdentificator;

            return View("_DisplayUserViewModelDataTable", usersViewModel);
        }
    }
}
