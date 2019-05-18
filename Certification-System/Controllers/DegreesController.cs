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

                if (degree.RequiredDegrees == null)
                {
                    degree.RequiredDegrees = new List<string>();
                }
                if (degree.RequiredCertificates == null)
                {
                    degree.RequiredCertificates = new List<string>();
                }

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

        // GET: AddNewDegreeConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewDegreeConfirmation(string degreeIdentificator)
        {
            if (degreeIdentificator != null)
            {
                var Degree = _context.GetDegreeById(degreeIdentificator);

                var RequiredCertificates = _context.GetCertificatesById(Degree.RequiredCertificates);
                var RequiredDegrees = _context.GetDegreesById(Degree.RequiredDegrees);
                var Branches = _context.GetBranchesById(Degree.Branches);

                DisplayDegreeViewModel addedDegree = new DisplayDegreeViewModel
                {
                    DegreeIndexer = Degree.DegreeIndexer,
                    Name = Degree.Name,
                    Description = Degree.Description,

                    RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList(),
                    RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIdentificator + " " + z.Name).ToList(),
                    Branches = Branches
                };

                return View(addedDegree);
            }
            return RedirectToAction(nameof(AddNewDegree));
        }
    }
}