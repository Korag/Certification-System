using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public CertificatesController()
        {
            _context = new MongoOperations();
        }

        // GET: BlankMenu
        [Authorize]
        public IActionResult BlankMenu()
        {
            return View();
        }

        // GET: AddNewCertificate
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewCertificate()
        {
            AddCertificateToDbViewModel newCertificate = new AddCertificateToDbViewModel
            {
                AvailableBranches = _context.GetBranchesAsSelectList().ToList(),
                SelectedBranches = new List<string>()
            };

            //newCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();

            return View(newCertificate);
        }

        // GET: AddNewCertificateConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewCertificateConfirmation(string certificateIdentificator)
        {
            if (certificateIdentificator != null)
            {
                var Certificate = _context.GetCertificateById(certificateIdentificator);

                AddCertificateToDbViewModel addedCertificate = new AddCertificateToDbViewModel
                {
                    CertificateIndexer = Certificate.CertificateIndexer,
                    Name = Certificate.Name,
                    Description = Certificate.Description,
                };

                var BranchNames = _context.GetBranchesById(Certificate.Branches);

                addedCertificate.SelectedBranches = BranchNames;

                return View(addedCertificate);
            }
            return RedirectToAction(nameof(AddNewCertificate));
        }

        // POST: AddNewCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewCertificate(AddCertificateToDbViewModel newCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = new Certificate
                {
                    CertificateIdentificator = ObjectId.GenerateNewId().ToString(),

                    CertificateIndexer = newCertificate.CertificateIndexer,
                    Name = newCertificate.Name,
                    Description = newCertificate.Description,

                    Depreciated = newCertificate.Depreciated,

                    Branches = newCertificate.SelectedBranches
                };

                _context.AddCertificate(certificate);

                return RedirectToAction("AddNewCertificateConfirmation", new { certificateIdentificator = certificate.CertificateIdentificator });
            }

            newCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (newCertificate.SelectedBranches == null)
            {
                newCertificate.SelectedBranches = new List<string>();
            }
            return View(newCertificate);
        }

        // GET: DisplayAllCertificates
        [Authorize(Roles = "Admin")]
        public IActionResult DisplayAllCertificates()
        {
            var Certificates = _context.GetCertificates();
            List<DisplayListOfCertificatesViewModel> ListOfCertificates = new List<DisplayListOfCertificatesViewModel>();

            foreach (var certificate in Certificates)
            {
                DisplayListOfCertificatesViewModel singleCertificate = new DisplayListOfCertificatesViewModel
                {
                    CertificateIndexer = certificate.CertificateIndexer,
                    Name = certificate.Name,
                    Description = certificate.Description,
                    Branches = _context.GetBranchesById(certificate.Branches),
                    Depreciated = certificate.Depreciated
                };

                ListOfCertificates.Add(singleCertificate);
            }

            return View(ListOfCertificates);
        }

        // GET: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewGivenCertificate()
        {
            AddNewGivenCertificateViewModel newGivenCertificate = new AddNewGivenCertificateViewModel
            {
                AvailableCertificates = _context.GetCertificatesAsSelectList().ToList(),
                AvailableUsers = _context.GetUsersAsSelectList().ToList(),
                AvailableCourses = _context.GetCoursesAsSelectList().ToList()
            };

            return View(newGivenCertificate);
        }

        // POST: AddNewGivenCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewGivenCertificate(AddNewGivenCertificateViewModel newGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                GivenCertificate givenCertificate = new GivenCertificate
                {
                    GivenCertificateIdentificator = ObjectId.GenerateNewId().ToString(),
                    GivenCertificateIndexer = newGivenCertificate.GivenCertificateIndexer,

                    ReceiptDate = newGivenCertificate.ReceiptDate,
                    ExpirationDate = newGivenCertificate.ExpirationDate,

                    Course = _context.GetCourseById(newGivenCertificate.SelectedCourses).CourseIdentificator,
                    Certificate = _context.GetCertificateById(newGivenCertificate.SelectedCertificate).CertificateIdentificator
                };

                _context.AddGivenCertificate(givenCertificate);
                _context.AddUserCertificate(newGivenCertificate.SelectedUser, givenCertificate.GivenCertificateIdentificator);

                return RedirectToAction("AddNewGivenCertificateConfirmation", new { givenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator });
            }

            newGivenCertificate.AvailableCertificates = _context.GetCertificatesAsSelectList().ToList();
            newGivenCertificate.AvailableUsers = _context.GetUsersAsSelectList().ToList();
            newGivenCertificate.AvailableCourses = _context.GetCoursesAsSelectList().ToList();

            return View(newGivenCertificate);
        }

        // GET: AddNewGivenCertificateConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewGivenCertificateConfirmation(string givenCertificateIdentificator)
        {
            if (givenCertificateIdentificator != null)
            {
                var GivenCertificate = _context.GetGivenCertificateById(givenCertificateIdentificator);

                var Course = _context.GetCourseById(GivenCertificate.Course);
                var Certificate = _context.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.GetUserCertificate(GivenCertificate.GivenCertificateIdentificator);

                DisplayGivenCertificateViewModel addedGivenCertificate = new DisplayGivenCertificateViewModel
                {
                    GivenCertificateIndexer = GivenCertificate.GivenCertificateIndexer,
                    ReceiptDate = GivenCertificate.ReceiptDate,
                    ExpirationDate = GivenCertificate.ExpirationDate,

                    Course = Course.CourseIndexer + " " + Course.Name,
                    Certificate = Certificate.CertificateIndexer + " " + Certificate.Name,
                    User = User.FirstName + " " + User.LastName        
                };


                return View(addedGivenCertificate);
            }
            return RedirectToAction(nameof(AddNewGivenCertificate));
        }
    }
}