using Certification_System.DAL;
using Certification_System.DTOViewModels;
using Certification_System.Entitities;
using Certification_System.ServicesInterfaces.IGeneratorQR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        public IDatabaseOperations _context { get; set; }
        public IGeneratorQR _generatorQR { get; set; }

        public CertificatesController(IGeneratorQR generatorQR)
        {
            _generatorQR = generatorQR;
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

        // GET: DisplayAllGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenCertificates()
        {
            var GivenCertificates = _context.GetGivenCertificates();
            List<DisplayGivenCertificateViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateViewModel>();

            foreach (var givenCertificate in GivenCertificates)
            {
                var Course = _context.GetCourseById(givenCertificate.Course);
                var Certificate = _context.GetCertificateById(givenCertificate.Certificate);
                var User = _context.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);

                DisplayListOfCoursesViewModel courseViewModel = new DisplayListOfCoursesViewModel
                {
                    CourseIdentificator = Course.CourseIdentificator,

                    CourseIndexer = Course.CourseIndexer,
                    Name = Course.Name,
                };

                DisplayListOfCertificatesViewModel certificateViewModel = new DisplayListOfCertificatesViewModel
                {
                    CertificateIdentificator = Certificate.CertificateIdentificator,

                    CertificateIndexer = Certificate.CertificateIndexer,
                    Name = Certificate.Name
                };

                DisplayUsersViewModel userViewModel = new DisplayUsersViewModel
                {
                    UserIdentificator = User.Id,

                    FirstName = User.FirstName,
                    LastName = User.LastName
                };

                DisplayGivenCertificateViewModel singleGivenCertificate = new DisplayGivenCertificateViewModel
                {
                    GivenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator,

                    GivenCertificateIndexer = givenCertificate.GivenCertificateIndexer,
                    ReceiptDate = givenCertificate.ReceiptDate,
                    ExpirationDate = givenCertificate.ExpirationDate,

                    Certificate = certificateViewModel,
                    Course = courseViewModel,
                    User = userViewModel
                };

                ListOfGivenCertificates.Add(singleGivenCertificate);
            }

            return View(ListOfGivenCertificates);
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

                return RedirectToAction("AddNewGivenCertificateConfirmation", new { givenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            newGivenCertificate.AvailableCertificates = _context.GetCertificatesAsSelectList().ToList();
            newGivenCertificate.AvailableUsers = _context.GetUsersAsSelectList().ToList();
            newGivenCertificate.AvailableCourses = _context.GetCoursesAsSelectList().ToList();

            return View(newGivenCertificate);
        }

        // GET: AddNewGivenCertificateConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenCertificateConfirmation(string givenCertificateIdentificator, string TypeOfAction)
        {
            if (givenCertificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var GivenCertificate = _context.GetGivenCertificateById(givenCertificateIdentificator);

                var Course = _context.GetCourseById(GivenCertificate.Course);
                var Certificate = _context.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

                DisplayUsersViewModel userViewModel = new DisplayUsersViewModel
                {
                    UserIdentificator = User.Id,

                    FirstName = User.FirstName,
                    LastName = User.LastName
                };

                DisplayListOfCoursesViewModel courseViewModel = new DisplayListOfCoursesViewModel
                {
                    CourseIdentificator = Course.CourseIdentificator,

                    CourseIndexer = Course.CourseIndexer,
                    Name = Course.Name
                };

                DisplayListOfCertificatesViewModel certificateViewModel = new DisplayListOfCertificatesViewModel
                {
                    CertificateIdentificator = Certificate.CertificateIdentificator,

                    CertificateIndexer = Certificate.CertificateIndexer,
                    Name = Certificate.Name
                };

                DisplayGivenCertificateViewModel addedGivenCertificate = new DisplayGivenCertificateViewModel
                {
                    GivenCertificateIndexer = GivenCertificate.GivenCertificateIndexer,
                    ReceiptDate = GivenCertificate.ReceiptDate,
                    ExpirationDate = GivenCertificate.ExpirationDate,

                    Course = courseViewModel,
                    Certificate = certificateViewModel,
                    User = userViewModel
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

                return RedirectToAction("AddNewCertificateConfirmation", "Certificates", new { certificateIdentificator = editedCertificate.CertificateIdentificator, TypeOfAction = "Update" });
            }

            editedCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (editedCertificate.SelectedBranches == null)
            {
                editedCertificate.SelectedBranches = new List<string>();
            }
            return View(editedCertificate);
        }

        // GET: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditGivenCertificate(string givenCertificateIdentificator)
        {
            var GivenCertificate = _context.GetGivenCertificateById(givenCertificateIdentificator);

            EditGivenCertificateViewModel givenCertificateToUpdate = new EditGivenCertificateViewModel
            {
                GivenCertificateIdentificator = GivenCertificate.GivenCertificateIdentificator,
                GivenCertificateIndexer = GivenCertificate.GivenCertificateIndexer,
                ReceiptDate = GivenCertificate.ReceiptDate,
                ExpirationDate = GivenCertificate.ExpirationDate
            };

            return View(givenCertificateToUpdate);
        }

        // POST: EditGivenCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenCertificate(EditGivenCertificateViewModel editedGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                var OriginGivenCertificate = _context.GetGivenCertificateById(editedGivenCertificate.GivenCertificateIdentificator);

                GivenCertificate givenCertificate = new GivenCertificate
                {
                    GivenCertificateIdentificator = OriginGivenCertificate.GivenCertificateIdentificator,

                    GivenCertificateIndexer = editedGivenCertificate.GivenCertificateIndexer,
                    ReceiptDate = editedGivenCertificate.ReceiptDate,
                    ExpirationDate = editedGivenCertificate.ExpirationDate,

                    Certificate = OriginGivenCertificate.Certificate,
                    Course = OriginGivenCertificate.Course
                };

                _context.UpdateGivenCertificate(givenCertificate);

                return RedirectToAction("AddNewGivenCertificateConfirmation", "Certificates", new { givenCertificateIdentificator = OriginGivenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenCertificate);
        }


        // GET: DegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult DegreeDetails(string certificateIdentificator)
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

        // GET: VerifyUserCompetencesByQR
        [AllowAnonymous]
        public ActionResult VerifyUserCompetencesByQR(string userIdentificator)
        {
            if (this.User.IsInRole("Admin"))
            {
                return RedirectToAction("UserDetails", "Users", new { userIdentificator = userIdentificator });
            }
            else
            {
                return RedirectToAction("AnonymousVerificationOfUser", "Users", new { userIdentificator = userIdentificator });
            }
        }

        // GET: VerifyUserCertificateByQR
        [AllowAnonymous]
        public ActionResult VerifyUserCertificateByQR(string givenCertificateIdentificator)
        {
            if (givenCertificateIdentificator != null)
            {
                var GivenCertificate = _context.GetGivenCertificateById(givenCertificateIdentificator);

                var Certificate = _context.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

                DisplayUsersViewModel userViewModel = new DisplayUsersViewModel
                {
                    UserIdentificator = User.Id,

                    FirstName = User.FirstName,
                    LastName = User.LastName
                };

                DisplayListOfCertificatesViewModel certificateViewModel = new DisplayListOfCertificatesViewModel
                {
                    CertificateIdentificator = Certificate.CertificateIdentificator,

                    CertificateIndexer = Certificate.CertificateIndexer,
                    Name = Certificate.Name
                };

                DisplayGivenCertificateViewModel VerifiedGivenCertificate = new DisplayGivenCertificateViewModel
                {
                    GivenCertificateIdentificator = GivenCertificate.GivenCertificateIdentificator,

                    GivenCertificateIndexer = GivenCertificate.GivenCertificateIndexer,
                    ReceiptDate = GivenCertificate.ReceiptDate,
                    ExpirationDate = GivenCertificate.ExpirationDate,

                    Certificate = certificateViewModel,
                    User = userViewModel
                };

                return View(VerifiedGivenCertificate);
            }

            return RedirectToAction("Login", "Account");
        }

        // GET: VerifyUserOrSingleCertificateManual
        [AllowAnonymous]
        public ActionResult VerifyUserOrSingleCertificateManual()
        {
            return View();
        }

        // GET: VerifyUser
        [AllowAnonymous]
        public ActionResult VerifyUser()
        {
            return View();
        }

        // GET: VerifyCertificate
        [AllowAnonymous]
        public ActionResult VerifyCertificate()
        {
            return View();
        }


        // POST: VerifyUser
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyUser(VerifyUserViewModel userToVerify)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("VerifyUserCompetencesByQR", "Certificates", new { userIdentificator = userToVerify.UserIdentificator });
            }
            return View(userToVerify);
        }

        // POST: VerifyCertificate
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyCertificate(VerifyCertificateViewModel certificateToVerify)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("VerifyUserCertificateByQR", "Certificates", new { givenCertificateIdentificator = certificateToVerify.CertificateIdentificator });
            }
            return View(certificateToVerify);
        }

        // GET: GenerateUserQR
        [AllowAnonymous]
        public ActionResult GenerateUserQR(string userIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/Certificates/VerifyUserCompetencesByQR?userIdentificator=" + $"{userIdentificator}";
            var QRBitmap = _generatorQR.GenerateQRCode(URL);

            using (MemoryStream stream = new MemoryStream())
            {
                QRBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var ByteArray = stream.ToArray();

                return File(ByteArray, "image/jpeg");
            }
        }
    }
}