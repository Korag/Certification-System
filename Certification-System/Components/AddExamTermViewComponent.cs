using Certification_System.DTOViewModels;
using Certification_System.Repository.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Certification_System.Components
{
    public class AddExamTermViewComponent : ViewComponent
    {
        private readonly MongoOperations _context;

        public AddExamTermViewComponent(MongoOperations context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(string orderNumber)
        {
            AddExamComponentViewModel model = new AddExamComponentViewModel
            {
               Iterator = Int32.Parse(orderNumber),
               AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList()
            };

            return View("_AddExamTerm", model);
        }
    }
}
