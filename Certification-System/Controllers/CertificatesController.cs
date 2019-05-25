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
        public ActionResult AddNewCertificate()
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
        public ActionResult AddNewCertificateConfirmation(string certificateIdentificator, string TypeOfAction)
        {
            if (certificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

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
        public ActionResult AddNewCertificate(AddCertificateToDbViewModel newCertificate)
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

                return RedirectToAction("AddNewCertificateConfirmation", new { certificateIdentificator = certificate.CertificateIdentificator, TypeOfAction = "Add" });
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
        public ActionResult DisplayAllCertificates()
        {
            var Certificates = _context.GetCertificates();
            List<DisplayListOfCertificatesViewModel> ListOfCertificates = new List<DisplayListOfCertificatesViewModel>();

            foreach (var certificate in Certificates)
            {
                DisplayListOfCertificatesViewModel singleCertificate = new DisplayListOfCertificatesViewModel
                {
                    CertificateIdentificator = certificate.CertificateIdentificator,
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
        public ActionResult AddNewGivenCertificate()
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
        public ActionResult AddNewGivenCertificate(AddNewGivenCertificateViewModel newGivenCertificate)
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
        public ActionResult AddNewGivenCertificateConfirmation(string givenCertificateIdentificator)
        {
            if (givenCertificateIdentificator != null)
            {
                var GivenCertificate = _context.GetGivenCertificateById(givenCertificateIdentificator);

                var Course = _context.GetCourseById(GivenCertificate.Course);
                var Certificate = _context.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

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


        // GET: EditCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditCertificate(string certificateIdentificator)
        {
            var Certificate = _context.GetCertificateById(certificateIdentificator);

            AddCertificateToDbViewModel certificateToUpdate = new AddCertificateToDbViewModel
            {
                CertificateIdentificator = Certificate.CertificateIdentificator,
                CertificateIndexer = Certificate.CertificateIndexer,
                Description = Certificate.Description,
                Name = Certificate.Name,
                Depreciated = Certificate.Depreciated,

                SelectedBranches = Certificate.Branches,
                AvailableBranches = _context.GetBranchesAsSelectList().ToList()
            };

            return View(certificateToUpdate);
        }


        // POST: EditCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCertificate(AddCertificateToDbViewModel editedCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = new Certificate
                {
                    CertificateIdentificator = editedCertificate.CertificateIdentificator,

                    CertificateIndexer = editedCertificate.CertificateIndexer,
                    Name = editedCertificate.Name,
                    Description = editedCertificate.Description,

                    Depreciated = editedCertificate.Depreciated,

                    Branches = editedCertificate.SelectedBranches
                };

                _context.UpdateCertificate(certificate);

                return RedirectToAction("AddNewCertificateConfirmation", "Certificates", new { CertificateIdentificator = editedCertificate.CertificateIdentificator, TypeOfAction = "Update" });
            }

            editedCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (editedCertificate.SelectedBranches == null)
            {
                editedCertificate.SelectedBranches = new List<string>();
            }
            return View(editedCertificate);
        }

        // GET: CertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CertificateDetails(string certificateIdentificator)
        {
            var Certificate = _context.GetCertificateById(certificateIdentificator);
            var GivenCertificatesInstances = _context.GetGivenCertificatesByIdOfCertificate(certificateIdentificator);

            var GivenCertificatesIdentificators = GivenCertificatesInstances.Select(z => z.GivenCertificateIdentificator);
            var UsersWithCertificate = _context.GetUsersByGivenCertificateId(GivenCertificatesIdentificators.ToList());

            List<DisplayUsersViewModel> ListOfUsers = new List<DisplayUsersViewModel>();

            if (UsersWithCertificate.Count != 0)
            {
                foreach (var user in UsersWithCertificate)
                {
                    DisplayUsersViewModel singleUser = new DisplayUsersViewModel
                    {
                        UserIdentificator = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        CompanyRoleWorker = user.CompanyRoleWorker,
                        CompanyRoleManager = user.CompanyRoleManager
                    };

                    ListOfUsers.Add(singleUser);
                }
            }

            var CoursesWhichEndedWithSuchCertificate = _context.GetCoursesById(GivenCertificatesInstances.Select(z => z.Course).ToList());

            List<DisplayListOfCoursesViewModel> ListOfCourses = new List<DisplayListOfCoursesViewModel>();

            if (CoursesWhichEndedWithSuchCertificate.Count != 0)
            {
                foreach (var course in CoursesWhichEndedWithSuchCertificate)
                {
                    DisplayListOfCoursesViewModel singleCourse = new DisplayListOfCoursesViewModel
                    {
                        CourseIdentificator = course.CourseIdentificator,
                        CourseIndexer = course.CourseIndexer,
                        Name = course.Name,
                        Description = course.Description,
                        DateOfStart = course.DateOfStart,
                        DateOfEnd = course.DateOfEnd,
                        CourseLength = course.CourseLength,
                        CourseEnded = course.CourseEnded,
                        EnrolledUsersLimit = course.EnrolledUsersLimit,
                        EnrolledUsersQuantity = course.EnrolledUsers.Count,

                        SelectedBranches = _context.GetBranchesById(course.Branches)
                    };

                    ListOfCourses.Add(singleCourse);
                }
            }

            CertificateDetailsViewModel CertificateDetails = new CertificateDetailsViewModel
            {
                CertificateIdentificator = Certificate.CertificateIdentificator,
                CertificateIndexer = Certificate.CertificateIndexer,
                Name = Certificate.Name,
                Description = Certificate.Description,
                Depreciated = Certificate.Depreciated,

                Branches = _context.GetBranchesById(Certificate.Branches),
                CoursesWhichEndedWithCertificate = ListOfCourses,
                UsersWithCertificate = ListOfUsers
            };

            return View(CertificateDetails);
        }
    }
}