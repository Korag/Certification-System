using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;


namespace Certification_System.Controllers
{
    public class DegreesController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public DegreesController()
        {
            _context = new MongoOperations();
        }

        // GET: AddNewDegree
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewDegree()
        {
            AddDegreeViewModel newDegree = new AddDegreeViewModel
            {
                AvailableBranches = _context.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.GetDegreesAsSelectList().ToList(),

                SelectedBranches = new List<string>()
            };

            return View(newDegree);
        }

        // POST: AddNewDegree
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewDegree(AddDegreeViewModel newDegree)
        {
            if (ModelState.IsValid)
            {
                Degree degree = new Degree
                {
                    DegreeIdentificator = ObjectId.GenerateNewId().ToString(),

                    DegreeIndexer = newDegree.DegreeIndexer,
                    Name = newDegree.Name,
                    Description = newDegree.Description,

                    RequiredCertificates = newDegree.SelectedCertificates,
                    RequiredDegrees = newDegree.SelectedDegrees,
                    Branches = newDegree.SelectedBranches
                };

                _context.AddDegree(degree);

                return RedirectToAction("AddNewDegreeConfirmation", new { degreeIdentificator = degree.DegreeIdentificator });
            }

            newDegree.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (newDegree.SelectedBranches == null)
            {
                newDegree.SelectedBranches = new List<string>();
            }
            return View(newDegree);
        }
    }
}