using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Certification_System.Controllers
{
    public class InstructorsController : Controller
    {
        private IDatabaseOperations _context;

        public InstructorsController()
        {
            _context = new MongoOperations();
        }

        // GET: DisplayAllInstructors
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllInstructors()
        {
            var Instructors = _context.GetInstructors();
            List<AddInstructorViewModel> DisplayInstructors = new List<AddInstructorViewModel>();

            foreach (var instructor in Instructors)
            {
                AddInstructorViewModel singleInstructor = new AddInstructorViewModel
                {
                    FirstName = instructor.FirstName,
                    LastName = instructor.LastName,
                    Email = instructor.Email,
                    Phone = instructor.Phone,
                    Country = instructor.Country,
                    City = instructor.City,
                    PostCode = instructor.PostCode,
                    Address = instructor.Address,
                    NumberOfApartment = instructor.NumberOfApartment
                };

                DisplayInstructors.Add(singleInstructor);
            }

            return View(DisplayInstructors);
        }


        // GET: AddNewInstructor
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewInstructor()
        {
            return View();
        }

        // GET: AddNewInstructorConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewInstructorConfirmation()
        {
      
        }

        // GET: AddNewInstructor
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewInstructor(AddInstructorViewModel newInstructor)
        {
            if (ModelState.IsValid)
            {
                Instructor instructor = new Instructor
                {
                    FirstName = newInstructor.FirstName,
                    LastName = newInstructor.LastName,
                    Email = newInstructor.Email,
                    Phone = newInstructor.Phone,
                    Country = newInstructor.Country,
                    City = newInstructor.City,
                    PostCode = newInstructor.PostCode,
                    Address = newInstructor.Address,
                    NumberOfApartment = newInstructor.NumberOfApartment
                };

                _context.AddInstructor(instructor);

                return RedirectToAction("AddNewInstructorConfirmation", new { companyName = newCompany.CompanyName });
            }

            return View(newInstructor);
        }
    }
}