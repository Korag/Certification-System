﻿using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using Certification_System.Extensions;

namespace Certification_System.Controllers
{
    public class BranchesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public BranchesController(
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            ILogService logger,
            IEmailSender emailSender)
        {
            _context = context;

            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: DisplayAllBranches
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllBranches(string message = null)
        {
            ViewBag.Message = message;

            var branchesList = _context.branchRepository.GetListOfBranches();
            List<DisplayBranchViewModel> existingBranches = _mapper.Map<List<DisplayBranchViewModel>>(branchesList);

            return View(existingBranches);
        }

        // GET: ConfirmationOfActionOnBranch
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnBranch(string branchIdentificator, string typeOfAction)
        {
            ViewBag.TypeOfAction = typeOfAction;

            var branch = _context.branchRepository.GetBranchById(branchIdentificator);

            if (branchIdentificator == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }

            DisplayBranchViewModel modifiedBranch = _mapper.Map<DisplayBranchViewModel>(branch);

            return View(modifiedBranch);
        }

        // GET: AddNewBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewBranch()
        {
            return View();
        }

        // POST: AddNewBranch
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewBranch(AddBranchViewModel newBranch)
        {
            if (ModelState.IsValid)
            {
                Branch branch = _mapper.Map<Branch>(newBranch);
                branch.BranchIdentificator = _keyGenerator.GenerateNewId();

                _context.branchRepository.AddBranch(branch);

                #region EntityLogs

                var logInfoAddBranch = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addBranch"]);
                _logger.AddBranchLog(branch, logInfoAddBranch);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddBranch = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addBranch"], "Nazwa: " + branch.Name);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddBranch);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches", new { branchIdentificator = branch.BranchIdentificator, typeOfAction = "Add" });
            }

            return View(newBranch);
        }

        // GET: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditBranch(string branchIdentificator)
        {
            var branch = _context.branchRepository.GetBranchById(branchIdentificator);
            EditBranchViewModel branchToUpdate = _mapper.Map<EditBranchViewModel>(branch);

            return View(branchToUpdate);
        }

        // POST: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditBranch(EditBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var originBranch = _context.branchRepository.GetBranchById(editedBranch.BranchIdentificator);

                originBranch = _mapper.Map<EditBranchViewModel, Branch>(editedBranch, originBranch);

                _context.branchRepository.UpdateBranch(originBranch);

                #region EntityLogs

                var logInfoUpdateBranch = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateBranch"]);
                _logger.AddBranchLog(originBranch, logInfoUpdateBranch);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateBranch = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateBranch"], "Nazwa: " + editedBranch.Name);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateBranch);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches", new { branchIdentificator = originBranch.BranchIdentificator, typeOfAction = "Update" });
            }

            return View(editedBranch);
        }

        // GET: DeleteBranchHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteBranchHub(string branchIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(branchIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteBranchEntityLink(branchIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteBranch(string branchIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(branchIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel branchToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = branchIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie obszaru certyfikacji"
                };

                return View("DeleteEntity", branchToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteBranch
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteBranch(DeleteEntityViewModel branchToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var branch = _context.branchRepository.GetBranchById(branchToDelete.EntityIdentificator);

            if (branch == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme)});
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, branchToDelete.Code))
            {
                _context.branchRepository.DeleteBranch(branchToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteBranch = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteBranch"]);

                var logInfoUpdateCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCertificate"]);
                var logInfoUpdateDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateDegree"]);
                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCourse"]);

                _logger.AddBranchLog(branch, logInfoDeleteBranch);

                var updatedCourses = _context.courseRepository.DeleteBranchFromCourses(branch.BranchIdentificator);
                _logger.AddCoursesLogs(updatedCourses, logInfoUpdateCourse);

                var updatedCertificates = _context.certificateRepository.DeleteBranchFromCertificates(branch.BranchIdentificator);
                _logger.AddCertificatesLogs(updatedCertificates, logInfoUpdateCertificate);

                var updatedDegrees = _context.degreeRepository.DeleteBranchFromDegrees(branch.BranchIdentificator);
                _logger.AddDegreesLogs(updatedDegrees, logInfoUpdateDegree);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonal = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteBranch"], "Nazwa: " + branch.Name);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonal);

                #endregion

                return RedirectToAction("DisplayAllBranches", "Branches", new { message = "Usunięto wskazany obszar certyfikacji" });
            }

            return View("DeleteEntity", branchToDelete);
        }
    }
}