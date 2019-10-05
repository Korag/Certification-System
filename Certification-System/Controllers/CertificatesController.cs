﻿using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IEmailSender _emailSender;
        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public CertificatesController(
            IGeneratorQR generatorQR,
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            ILogService logger,
            IEmailSender emailSender)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: BlankMenu
        [Authorize]
        public ActionResult BlankMenu(string message = null)
        {
            ViewBag.message = message;

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
                certificate.CertificateIndexer = _keyGenerator.GenerateCertificateEntityIndexer(certificate.Name);

                _context.certificateRepository.AddCertificate(certificate);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCertificateLog(certificate, logInfo);

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

                var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);
                DisplayCertificateViewModel modifiedCertificate = _mapper.Map<DisplayCertificateViewModel>(certificate);

                modifiedCertificate.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);

                return View(modifiedCertificate);
            }

            return RedirectToAction(nameof(AddNewCertificate));
        }

        // GET: DisplayAllCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCertificates(string message = null)
        {
            ViewBag.Message = message;

            var certificates = _context.certificateRepository.GetListOfCertificates();

            List<DisplayCertificateViewModel> listOfCertificates = _mapper.Map<List<DisplayCertificateViewModel>>(certificates);
            listOfCertificates.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

            return View(listOfCertificates);
        }

        // GET: EditCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditCertificate(string certificateIdentificator)
        {
            var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            EditCertificateViewModel certificateToUpdate = _mapper.Map<EditCertificateViewModel>(certificate);
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
                var originCertificate = _context.certificateRepository.GetCertificateById(editedCertificate.CertificateIdentificator);
                originCertificate = _mapper.Map<EditCertificateViewModel, Certificate>(editedCertificate, originCertificate);

                _context.certificateRepository.UpdateCertificate(originCertificate);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCertificateLog(originCertificate, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCertificate", "Certificates", new { certificateIdentificator = editedCertificate.CertificateIdentificator, TypeOfAction = "Update" });
            }

            editedCertificate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
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
            var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            var givenCertificatesInstances = _context.givenCertificateRepository.GetGivenCertificatesByIdOfCertificate(certificateIdentificator);
            var givenCertificatesIdentificators = givenCertificatesInstances.Select(z => z.GivenCertificateIdentificator);

            var usersWithCertificate = _context.userRepository.GetUsersByGivenCertificateId(givenCertificatesIdentificators.ToList());

            List<DisplayCrucialDataWithCompaniesRoleUserViewModel> listOfUsers = new List<DisplayCrucialDataWithCompaniesRoleUserViewModel>();

            if (usersWithCertificate.Count != 0)
            {
                foreach (var user in usersWithCertificate)
                {
                    DisplayCrucialDataWithCompaniesRoleUserViewModel singleUser = _mapper.Map<DisplayCrucialDataWithCompaniesRoleUserViewModel>(user);
                    singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                    singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                    listOfUsers.Add(singleUser);
                }
            }

            var coursesWhichEndedWithSuchCertificate = _context.courseRepository.GetCoursesById(givenCertificatesInstances.Select(z => z.Course).ToList());

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (coursesWhichEndedWithSuchCertificate.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(coursesWhichEndedWithSuchCertificate);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
            }

            CertificateDetailsViewModel certificateDetails = _mapper.Map<CertificateDetailsViewModel>(certificate);
            certificateDetails.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);
            certificateDetails.CoursesWhichEndedWithCertificate = listOfCourses;
            certificateDetails.UsersWithCertificate = listOfUsers;

            return View(certificateDetails);
        }

        // GET: DeleteCertificateHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCertificateHub(string certificateIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(certificateIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteCertificateEntityLink(certificateIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteCertificate
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCertificate(string certificateIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(certificateIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel certificateToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = certificateIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie certyfikatu"
                };

                return View("DeleteEntity", certificateToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteCertificate(DeleteEntityViewModel certificateToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var certificate = _context.certificateRepository.GetCertificateById(certificateToDelete.EntityIdentificator);

            if (certificate == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, certificateToDelete.Code))
            {
                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.certificateRepository.DeleteCertificate(certificateToDelete.EntityIdentificator);
                _logger.AddCertificateLog(certificate, logInfo);

                var updatedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificatesByCertificateId(certificateToDelete.EntityIdentificator);
                _logger.AddGivenCertificatesLogs(updatedGivenCertificates, logInfoUpdate);

                var updatedGivenDegrees = _context.degreeRepository.DeleteCertificateFromDegrees(certificateToDelete.EntityIdentificator);
                _logger.AddDegreesLogs(updatedGivenDegrees, logInfoUpdate);

                return RedirectToAction("DisplayAllCertificates", "Certificates", new { message = "Usunięto wskazany certyfikat" });
            }

            return View("DeleteEntity", certificateToDelete);
        }
    }
}