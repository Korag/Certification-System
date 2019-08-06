﻿using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public CertificatesController(IGeneratorQR generatorQR, MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: BlankMenu
        [Authorize]
        public ActionResult BlankMenu()
        {
            return View();
        }

        // GET: AddNewCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCertificate()
        {
            AddCertificateViewModel newCertificate = new AddCertificateViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                SelectedBranches = new List<string>()
            };

            return View(newCertificate);
        }

        // POST: AddNewCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCertificate(AddCertificateViewModel newCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = _mapper.Map<Certificate>(newCertificate);
                certificate.CertificateIdentificator = _keyGenerator.GenerateNewId();

                _context.certificateRepository.AddCertificate(certificate);

                return RedirectToAction("ConfirmationOfActionOnCertificate", new { certificateIdentificator = certificate.CertificateIdentificator, TypeOfAction = "Add" });
            }

            newCertificate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (newCertificate.SelectedBranches == null)
            {
                newCertificate.SelectedBranches = new List<string>();
            }

            return View(newCertificate);
        }

        // GET: ConfirmationOfActionOnCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCertificate(string certificateIdentificator, string TypeOfAction)
        {
            if (certificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);
                DisplayCertificateViewModel modifiedCertificate = _mapper.Map<DisplayCertificateViewModel>(Certificate);

                modifiedCertificate.Branches = _context.branchRepository.GetBranchesById(Certificate.Branches);

                return View(modifiedCertificate);
            }

            return RedirectToAction(nameof(AddNewCertificate));
        }

        // GET: DisplayAllCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCertificates()
        {
            var Certificates = _context.certificateRepository.GetCertificates();

            List<DisplayCertificateViewModel> ListOfCertificates = _mapper.Map<List<DisplayCertificateViewModel>>(Certificates);
            ListOfCertificates.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

            return View(ListOfCertificates);
        }

        // GET: DisplayAllGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenCertificates()
        {
            var GivenCertificates = _context.givenCertificateRepository.GetGivenCertificates();
            List<DisplayGivenCertificateViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateViewModel>();

            foreach (var givenCertificate in GivenCertificates)
            {
                var Course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var User = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataCourseViewModel courseViewModel  = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);
                DisplayCrucialDataUsersViewModel userViewModel = _mapper.Map<DisplayCrucialDataUsersViewModel>(User);

                DisplayGivenCertificateViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(givenCertificate);
                singleGivenCertificate.Certificate = certificateViewModel;
                singleGivenCertificate.Course = courseViewModel;
                singleGivenCertificate.User = userViewModel;

                ListOfGivenCertificates.Add(singleGivenCertificate);
            }

            return View(ListOfGivenCertificates);
        }

        // GET: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenCertificate()
        {
            AddGivenCertificateViewModel newGivenCertificate = new AddGivenCertificateViewModel
            {
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList(),
                AvailableCourses = _context.courseRepository.GetCoursesAsSelectList().ToList()
            };

            return View(newGivenCertificate);
        }

        // POST: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewGivenCertificate(AddGivenCertificateViewModel newGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                GivenCertificate givenCertificate = _mapper.Map<GivenCertificate>(newGivenCertificate);
                givenCertificate.GivenCertificateIdentificator = _keyGenerator.GenerateNewId();
                givenCertificate.Course = _context.courseRepository.GetCourseById(newGivenCertificate.SelectedCourses).CourseIdentificator;
                givenCertificate.Certificate = _context.certificateRepository.GetCertificateById(newGivenCertificate.SelectedCertificate).CertificateIdentificator;

                _context.givenCertificateRepository.AddGivenCertificate(givenCertificate);
                _context.userRepository.AddUserCertificate(newGivenCertificate.SelectedUser, givenCertificate.GivenCertificateIdentificator);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", new { givenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            newGivenCertificate.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            newGivenCertificate.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();
            newGivenCertificate.AvailableCourses = _context.courseRepository.GetCoursesAsSelectList().ToList();

            return View(newGivenCertificate);
        }

        // GET: ConfirmationOfActionOnGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnGivenCertificate(string givenCertificateIdentificator, string TypeOfAction)
        {
            if (givenCertificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

                var Course = _context.courseRepository.GetCourseById(GivenCertificate.Course);
                var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.userRepository.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataUsersViewModel userViewModel = _mapper.Map<DisplayCrucialDataUsersViewModel>(User);
                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                DisplayGivenCertificateViewModel modifiedGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(GivenCertificate);
                modifiedGivenCertificate.Course = courseViewModel;
                modifiedGivenCertificate.Certificate = certificateViewModel;
                modifiedGivenCertificate.User = userViewModel;

                return View(modifiedGivenCertificate);
            }

            return RedirectToAction(nameof(AddNewGivenCertificate));
        }

        // GET: EditCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditCertificate(string certificateIdentificator)
        {
            var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            EditCertificateViewModel certificateToUpdate = _mapper.Map<EditCertificateViewModel>(Certificate);
            certificateToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            return View(certificateToUpdate);
        }

        // POST: EditCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCertificate(EditCertificateViewModel editedCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = _mapper.Map<Certificate>(editedCertificate);
                _context.certificateRepository.UpdateCertificate(certificate);

                return RedirectToAction("ConfirmationOfActionOnCertificate", "Certificates", new { certificateIdentificator = editedCertificate.CertificateIdentificator, TypeOfAction = "Update" });
            }

            editedCertificate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
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
            var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            EditGivenCertificateViewModel givenCertificateToUpdate = _mapper.Map<EditGivenCertificateViewModel>(GivenCertificate);

            return View(givenCertificateToUpdate);
        }

        // POST: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenCertificate(EditGivenCertificateViewModel editedGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                var OriginGivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(editedGivenCertificate.GivenCertificateIdentificator);

                OriginGivenCertificate = _mapper.Map<EditGivenCertificateViewModel, GivenCertificate>(editedGivenCertificate, OriginGivenCertificate);
                _context.givenCertificateRepository.UpdateGivenCertificate(OriginGivenCertificate);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", "Certificates", new { givenCertificateIdentificator = OriginGivenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenCertificate);
        }

        // GET: CertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CertificateDetails(string certificateIdentificator)
        {
            var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            var GivenCertificatesInstances = _context.givenCertificateRepository.GetGivenCertificatesByIdOfCertificate(certificateIdentificator);
            var GivenCertificatesIdentificators = GivenCertificatesInstances.Select(z => z.GivenCertificateIdentificator);

            var UsersWithCertificate = _context.userRepository.GetUsersByGivenCertificateId(GivenCertificatesIdentificators.ToList());

            List<DisplayCrucialDataWithCompaniesRoleViewModel> ListOfUsers = new List<DisplayCrucialDataWithCompaniesRoleViewModel>();

            if (UsersWithCertificate.Count != 0)
            {
                foreach (var user in UsersWithCertificate)
                {
                    DisplayCrucialDataWithCompaniesRoleViewModel singleUser = _mapper.Map<DisplayCrucialDataWithCompaniesRoleViewModel>(user);
                    singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                    singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                    ListOfUsers.Add(singleUser);
                }
            }

            var CoursesWhichEndedWithSuchCertificate = _context.courseRepository.GetCoursesById(GivenCertificatesInstances.Select(z => z.Course).ToList());

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (CoursesWhichEndedWithSuchCertificate.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(CoursesWhichEndedWithSuchCertificate);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
            }

            CertificateDetailsViewModel CertificateDetails = _mapper.Map<CertificateDetailsViewModel>(Certificate);
            CertificateDetails.Branches = _context.branchRepository.GetBranchesById(Certificate.Branches);
            CertificateDetails.CoursesWhichEndedWithCertificate = ListOfCourses;
            CertificateDetails.UsersWithCertificate = ListOfUsers;

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
                var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

                var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.userRepository.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataUsersViewModel userViewModel = _mapper.Map<DisplayCrucialDataUsersViewModel>(User);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                DisplayGivenCertificateViewModel VerifiedGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(GivenCertificate);
                VerifiedGivenCertificate.Certificate = certificateViewModel;
                VerifiedGivenCertificate.User = userViewModel;

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