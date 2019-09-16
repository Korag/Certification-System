using Certification_System.Entities;
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

            var BranchesList = _context.branchRepository.GetListOfBranches();
            List<DisplayBranchViewModel> existingBranches = _mapper.Map<List<DisplayBranchViewModel>>(BranchesList);

            return View(existingBranches);
        }

        // GET: ConfirmationOfActionOnBranch
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnBranch(string branchIdentificator, string typeOfAction)
        {
            ViewBag.TypeOfAction = typeOfAction;

            var Branch = _context.branchRepository.GetBranchById(branchIdentificator);

            if (branchIdentificator == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }

            DisplayBranchViewModel modifiedBranch = _mapper.Map<DisplayBranchViewModel>(Branch);

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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddBranchLog(branch, logInfo);

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches", new { branchIdentificator = branch.BranchIdentificator, typeOfAction = "Add" });
            }

            return View(newBranch);
        }

        // GET: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditBranch(string branchIdentificator)
        {
            var Branch = _context.branchRepository.GetBranchById(branchIdentificator);
            EditBranchViewModel branchToUpdate = _mapper.Map<EditBranchViewModel>(Branch);

            return View(branchToUpdate);
        }

        // POST: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditBranch(EditBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var OriginBranch = _context.branchRepository.GetBranchById(editedBranch.BranchIdentificator);

                OriginBranch = _mapper.Map<EditBranchViewModel, Branch>(editedBranch, OriginBranch);

                _context.branchRepository.UpdateBranch(OriginBranch);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddBranchLog(OriginBranch, logInfo);

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches", new { branchIdentificator = OriginBranch.BranchIdentificator, typeOfAction = "Update" });
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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _logger.AddBranchLog(branch, logInfo);

                var updatedCourses = _context.courseRepository.DeleteBranchFromCourses(branch.BranchIdentificator);
                _logger.AddCoursesLogs(updatedCourses, logInfoUpdate);

                var updatedCertificates = _context.certificateRepository.DeleteBranchFromCertificates(branch.BranchIdentificator);
                _logger.AddCertificatesLogs(updatedCertificates, logInfoUpdate);

                var updatedDegrees = _context.degreeRepository.DeleteBranchFromDegrees(branch.BranchIdentificator);
                _logger.AddDegreesLogs(updatedDegrees, logInfoUpdate);

                return RedirectToAction("DisplayAllBranches", "Branches", new { message = "Usunięto wskazany obszar certyfikacji" });
            }

            return View("DeleteEntity", branchToDelete);
        }
    }
}