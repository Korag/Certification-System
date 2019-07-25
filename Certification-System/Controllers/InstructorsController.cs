using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using Certification_System.Repository.DAL;

namespace Certification_System.Controllers
{
    public class InstructorsController : Controller
    {
        private MongoOperations _context;

        public InstructorsController(MongoOperations context)
        {
            _context = context;
        }

        // GET: DisplayAllInstructors
        [Authorize(Roles = "Admin")]
        public IActionResult DisplayAllInstructors()
        {
            var Instructors = _context.instructorRepository.GetInstructors();
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
        public IActionResult AddNewInstructor()
        {
            return View();
        }

        // GET: AddNewInstructorConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewInstructorConfirmation(string instructorIdentificator)
        {
            if (instructorIdentificator != null)
            {
                Instructor instructor = _context.instructorRepository.GetInstructorById(instructorIdentificator);

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

                return View(singleInstructor);
            }

            return RedirectToAction(nameof(AddNewInstructor));
        }

        // GET: AddNewInstructor
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewInstructor(AddInstructorViewModel newInstructor)
        {
            if (ModelState.IsValid)
            {
                Instructor instructor = new Instructor
                {
                    InstructorIdentificator = ObjectId.GenerateNewId().ToString(),
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

                _context.instructorRepository.AddInstructor(instructor);

                return RedirectToAction("AddNewInstructorConfirmation", new { instructorIdentificator = instructor.InstructorIdentificator });
            }

            return View(newInstructor);
        }
    }
}