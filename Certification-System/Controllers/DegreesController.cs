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

                _context.degreeRepository.AddDegree(degree);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddDegreeLog(degree, logInfo);

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

                var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

                var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);
                var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
                var Branches = _context.branchRepository.GetBranchesById(Degree.Branches);

                DisplayDegreeViewModel modifiedDegree = _mapper.Map<DisplayDegreeViewModel>(Degree);

                modifiedDegree.RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                modifiedDegree.RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                modifiedDegree.Branches = Branches;

                return View(modifiedDegree);
            }

            return RedirectToAction(nameof(AddNewDegree));
        }

        // GET: DisplayAllDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllDegrees(string message = null)
        {
            ViewBag.Message = message;

            var Degrees = _context.degreeRepository.GetListOfDegrees();
            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            foreach (var degree in Degrees)
            {
                var RequiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);
                var RequiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);

                DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);

                singleDegree.RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                singleDegree.RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);


                ListOfDegrees.Add(singleDegree);
            }

            return View(ListOfDegrees);
        }

        // GET: EditDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditDegree(string degreeIdentificator)
        {
            var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            EditDegreeViewModel degreeViewModel = _mapper.Map<EditDegreeViewModel>(Degree);

            degreeViewModel.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            degreeViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            degreeViewModel.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();

            return View(degreeViewModel);
        }

        // POST: EditDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditDegree(EditDegreeViewModel editedDegree)
        {
            if (ModelState.IsValid)
            {
                var OriginDegree = _context.degreeRepository.GetDegreeById(editedDegree.DegreeIdentificator);
                OriginDegree =_mapper.Map<EditDegreeViewModel, Degree>(editedDegree, OriginDegree);

                _context.degreeRepository.UpdateDegree(OriginDegree);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddDegreeLog(OriginDegree, logInfo);

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
            var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
            var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);

            var GivenDegreesInstances = _context.givenDegreeRepository.GetGivenDegreesByIdOfDegree(degreeIdentificator);
            var GivenDegreesIdentificators = GivenDegreesInstances.Select(z => z.GivenDegreeIdentificator);
            var UsersWithDegree = _context.userRepository.GetUsersByDegreeId(GivenDegreesIdentificators.ToList());

            List<DisplayCertificateViewModel> ListOfCertificates = new List<DisplayCertificateViewModel>();

            if (RequiredCertificates.Count != 0)
            {
                foreach (var certificate in RequiredCertificates)
                {
                    DisplayCertificateViewModel singleCertificate = _mapper.Map<DisplayCertificateViewModel>(certificate);
                    singleCertificate.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);

                    ListOfCertificates.Add(singleCertificate);
                }
            }

            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            if (RequiredDegrees.Count != 0)
            {
                foreach (var degree in RequiredDegrees)
                {
                    DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);
                    singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);
                    singleDegree.RequiredCertificates = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees).Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                    singleDegree.RequiredDegrees = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates).Select(z => z.CertificateIndexer + " " + z.Name).ToList();

                    ListOfDegrees.Add(singleDegree);
                }
            }

            List<DisplayUserViewModel> ListOfUsers = new List<DisplayUserViewModel>();

            foreach (var user in UsersWithDegree)
            {
                DisplayUserViewModel singleUser = _mapper.Map<DisplayUserViewModel>(user);
                singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                ListOfUsers.Add(singleUser);
            }

            DegreeDetailsViewModel DegreeDetails = _mapper.Map<DegreeDetailsViewModel>(Degree);
            DegreeDetails.Branches = _context.branchRepository.GetBranchesById(Degree.Branches);
            DegreeDetails.RequiredCertificates = ListOfCertificates;
            DegreeDetails.RequiredDegrees = ListOfDegrees;
            DegreeDetails.UsersWithDegree = ListOfUsers;

            return View(DegreeDetails);
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
                var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.degreeRepository.DeleteDegree(degreeToDelete.EntityIdentificator);
                _logger.AddDegreeLog(degree, logInfoDelete);

                var deletedGivenDegrees = _context.givenDegreeRepository.DeleteGivenDegreesByDegreeId(degreeToDelete.EntityIdentificator);
                _logger.AddGivenDegreesLogs(deletedGivenDegrees, logInfoDelete);

                var updatedDegrees = _context.degreeRepository.DeleteRequiredDegreeFromDegree(degreeToDelete.EntityIdentificator);
                _logger.AddDegreesLogs(updatedDegrees, logInfoUpdate);

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