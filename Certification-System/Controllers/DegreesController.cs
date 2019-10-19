using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using Certification_System.Extensions;
using System;

namespace Certification_System.Controllers
{
    public class DegreesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public DegreesController(
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

        // GET: AddNewDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewDegree()
        {
            AddDegreeViewModel newDegree = new AddDegreeViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList(),

                SelectedBranches = new List<string>()
            };

            return View(newDegree);
        }

        // POST: AddNewDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewDegree(AddDegreeViewModel newDegree)
        {
            if (ModelState.IsValid)
            {
                Degree degree = _mapper.Map<Degree>(newDegree);
                degree.DegreeIdentificator = _keyGenerator.GenerateNewId();
                degree.DegreeIndexer = _keyGenerator.GenerateDegreeEntityIndexer(degree.Name);

                degree.Conditions = newDegree.ConditionsList.Split(",").ToList();

                _context.degreeRepository.AddDegree(degree);

                var logInfoAddDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptionOfAction.DescriptionOfActions["addDegree"]);
                _logger.AddDegreeLog(degree, logInfoAddDegree);

                return RedirectToAction("ConfirmationOfActionOnDegree", new { degreeIdentificator = degree.DegreeIdentificator, TypeOfAction = "Add" });
            }

            newDegree.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (newDegree.SelectedBranches == null)
            {
                newDegree.SelectedBranches = new List<string>();
            }

            return View(newDegree);
        }

        // GET: ConfirmationOfActionOnDegree
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnDegree(string degreeIdentificator, string TypeOfAction)
        {
            if (degreeIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

                var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);
                var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
                var branches = _context.branchRepository.GetBranchesById(degree.Branches);

                DisplayDegreeViewModel modifiedDegree = _mapper.Map<DisplayDegreeViewModel>(degree);

                modifiedDegree.RequiredCertificates = requiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                modifiedDegree.RequiredDegrees = requiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                modifiedDegree.Branches = branches;

                return View(modifiedDegree);
            }

            return RedirectToAction(nameof(AddNewDegree));
        }

        // GET: DisplayAllDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllDegrees(string message = null)
        {
            ViewBag.Message = message;

            var degrees = _context.degreeRepository.GetListOfDegrees();
            List<DisplayDegreeViewModel> listOfDegrees = new List<DisplayDegreeViewModel>();

            foreach (var degree in degrees)
            {
                var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);
                var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);

                DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);

                singleDegree.RequiredCertificates = requiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                singleDegree.RequiredDegrees = requiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);


                listOfDegrees.Add(singleDegree);
            }

            return View(listOfDegrees);
        }

        // GET: EditDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditDegree(string degreeIdentificator)
        {
            var degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            EditDegreeViewModel degreeViewModel = _mapper.Map<EditDegreeViewModel>(degree);

            degreeViewModel.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            degreeViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            degreeViewModel.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();

            degreeViewModel.ConditionsList = String.Join(",", degree.Conditions); 

            return View(degreeViewModel);
        }

        // POST: EditDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditDegree(EditDegreeViewModel editedDegree)
        {
            if (ModelState.IsValid)
            {
                var originDegree = _context.degreeRepository.GetDegreeById(editedDegree.DegreeIdentificator);
                originDegree = _mapper.Map<EditDegreeViewModel, Degree>(editedDegree, originDegree);

                originDegree.Conditions = editedDegree.ConditionsList.Split(",").ToList();

                _context.degreeRepository.UpdateDegree(originDegree);

                var logInfoUpdateDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["updateDegree"]);
                _logger.AddDegreeLog(originDegree, logInfoUpdateDegree);

                return RedirectToAction("ConfirmationOfActionOnDegree", "Degrees", new { degreeIdentificator = editedDegree.DegreeIdentificator, TypeOfAction = "Update" });
            }

            editedDegree.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            editedDegree.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            editedDegree.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();

            return View(editedDegree);
        }

        // GET: DegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult DegreeDetails(string degreeIdentificator)
        {
            var degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
            var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);

            var givenDegreesInstances = _context.givenDegreeRepository.GetGivenDegreesByIdOfDegree(degreeIdentificator);
            var givenDegreesIdentificators = givenDegreesInstances.Select(z => z.GivenDegreeIdentificator);
            var usersWithDegree = _context.userRepository.GetUsersByDegreeId(givenDegreesIdentificators.ToList());

            List<DisplayCertificateViewModel> listOfCertificates = new List<DisplayCertificateViewModel>();

            if (requiredCertificates.Count != 0)
            {
                foreach (var certificate in requiredCertificates)
                {
                    DisplayCertificateViewModel singleCertificate = _mapper.Map<DisplayCertificateViewModel>(certificate);
                    singleCertificate.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);

                    listOfCertificates.Add(singleCertificate);
                }
            }

            List<DisplayDegreeViewModel> listOfDegrees = new List<DisplayDegreeViewModel>();

            if (requiredDegrees.Count != 0)
            {
                foreach (var requiredSingleDegree in requiredDegrees)
                {
                    DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(requiredSingleDegree);
                    singleDegree.Branches = _context.branchRepository.GetBranchesById(requiredSingleDegree.Branches);
                    singleDegree.RequiredCertificates = _context.degreeRepository.GetDegreesById(requiredSingleDegree.RequiredDegrees).Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                    singleDegree.RequiredDegrees = _context.certificateRepository.GetCertificatesById(requiredSingleDegree.RequiredCertificates).Select(z => z.CertificateIndexer + " " + z.Name).ToList();

                    listOfDegrees.Add(singleDegree);
                }
            }

            List<DisplayUserViewModel> listOfUsers = new List<DisplayUserViewModel>();

            foreach (var user in usersWithDegree)
            {
                DisplayUserViewModel singleUser = _mapper.Map<DisplayUserViewModel>(user);
                singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                listOfUsers.Add(singleUser);
            }

            DegreeDetailsViewModel degreeDetails = _mapper.Map<DegreeDetailsViewModel>(degree);
            degreeDetails.ConditionsList = String.Join(",", degree.Conditions);

            degreeDetails.Branches = _context.branchRepository.GetBranchesById(degree.Branches);
            degreeDetails.RequiredCertificates = listOfCertificates;
            degreeDetails.RequiredDegrees = listOfDegrees;
            degreeDetails.UsersWithDegree = listOfUsers;

            return View(degreeDetails);
        }

        // GET: DeleteDegreeHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteDegreeHub(string degreeIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(degreeIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteDegreeEntityLink(degreeIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteDegree
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteDegree(string degreeIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(degreeIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel degreeToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = degreeIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie stopnia zawodowego"
                };

                return View("DeleteEntity", degreeToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteDegree(DeleteEntityViewModel degreeToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var degree = _context.degreeRepository.GetDegreeById(degreeToDelete.EntityIdentificator);

            if (degree == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, degreeToDelete.Code))
            {
                var logInfoDeleteDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptionOfAction.DescriptionOfActions["deleteDegree"]);
                var logInfoDeleteGivenDegrees = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["deleteGivenDegree"]);
                var logInfoDeleteRequiredDegreeFromGivenDegrees = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptionOfAction.DescriptionOfActions["deleteRequiredDegreeFromDegree"]);

                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["deleteUserGivenDegree"]);

                _context.degreeRepository.DeleteDegree(degreeToDelete.EntityIdentificator);
                _logger.AddDegreeLog(degree, logInfoDeleteDegree);

                var deletedRequiredDegrees = _context.degreeRepository.DeleteRequiredDegreeFromDegree(degreeToDelete.EntityIdentificator);
                _logger.AddDegreesLogs(deletedRequiredDegrees, logInfoDeleteDegree);

                var deletedGivenDegrees = _context.givenDegreeRepository.DeleteGivenDegreesByDegreeId(degreeToDelete.EntityIdentificator);
                _logger.AddGivenDegreesLogs(deletedGivenDegrees, logInfoDeleteGivenDegrees);

                var updatedUsers = _context.userRepository.GetUsersByGivenDegreesId(deletedGivenDegrees.Select(z=> z.GivenDegreeIdentificator).ToList());
                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);

                return RedirectToAction("DisplayAllDegrees", "Degrees", new { message = "Usunięto wskazany stopień zawodowy" });
            }

            return View("DeleteEntity", degreeToDelete);
        }

        #region AjaxQuery
        // GET: GetAvailableDegreesToDisposeByUserId
        [Authorize(Roles = "Admin")]
        public string[][] GetAvailableDegreesToDisposeByUserId(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var userGivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
            var userCertificatesTypes = userGivenCertificates.Select(z => z.Certificate).ToList();

            var userGivenDegree = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);
            var userDegreesTypes = userGivenDegree.Select(z => z.Degree).ToList();

            var availableGivenDegreesToDispose = _context.degreeRepository.GetDegreesToDisposeByUserCompetences(userCertificatesTypes, userDegreesTypes).ToList();

            string[][] givenDegreesArray = new string[availableGivenDegreesToDispose.Count()][];

            for (int i = 0; i < givenDegreesArray.Count(); i++)
            {
                givenDegreesArray[i] = new string[2];

                givenDegreesArray[i][0] = availableGivenDegreesToDispose[i].DegreeIdentificator;
                givenDegreesArray[i][1] = availableGivenDegreesToDispose[i].DegreeIndexer + " | " + availableGivenDegreesToDispose[i].Name;
            }

            return givenDegreesArray;
        }
        #endregion
    }
}