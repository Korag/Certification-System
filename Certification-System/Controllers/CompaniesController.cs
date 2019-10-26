using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using System.Linq;
using Certification_System.Extensions;

namespace Certification_System.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public CompaniesController(
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

        // GET: DisplayAllCompanies
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCompanies(string message = null)
        {
            ViewBag.Message = message;

            var companies = _context.companyRepository.GetListOfCompanies();
            List<DisplayCompanyViewModel> listOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            return View(listOfCompanies);
        }

        // GET: AddNewCompany
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCompany()
        {
            return View();
        }

        // GET: ConfirmationOfActionOnCompany
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCompany(string companyIdentificator, string TypeOfAction)
        {
            if (companyIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                Company company = _context.companyRepository.GetCompanyById(companyIdentificator);
                DisplayCompanyViewModel modifiedCompany = _mapper.Map<DisplayCompanyViewModel>(company);

                return View(modifiedCompany);
            }

            return RedirectToAction(nameof(AddNewCompany));
        }

        // GET: AddNewCompany
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCompany(AddCompanyViewModel newCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = _mapper.Map<Company>(newCompany);
                company.CompanyIdentificator = _keyGenerator.GenerateNewId();

                _context.companyRepository.AddCompany(company);

                var logInfoAddCompany = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addCompany"]);
                _logger.AddCompanyLog(company, logInfoAddCompany);

                var logInfoPersonalAddCompany = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addCompany"], "Nazwa przedsiębiorstwa: " + company.CompanyName);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddCompany);

                return RedirectToAction("ConfirmationOfActionOnCompany", new { companyIdentificator = company.CompanyIdentificator, TypeOfAction = "Add" });
            }

            return View(newCompany);
        }

        // GET: EditCompany
        [Authorize(Roles = "Admin")]
        public ActionResult EditCompany(string companyIdentificator)
        {
            var company = _context.companyRepository.GetCompanyById(companyIdentificator);
            EditCompanyViewModel companyToUpdate = _mapper.Map<EditCompanyViewModel>(company);

            return View(companyToUpdate);
        }

        // POST: EditCompany
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCompany(EditCompanyViewModel editedCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = _mapper.Map<Company>(editedCompany);
                _context.companyRepository.UpdateCompany(company);

                var logInfoUpdateCompany = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCompany"]);
                _logger.AddCompanyLog(company, logInfoUpdateCompany);

                var logInfoPersonalUpdateCompany = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateCompany"], "Nazwa przedsiębiorstwa: " + company.CompanyName);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateCompany);

                return RedirectToAction("ConfirmationOfActionOnCompany", "Companies", new { companyIdentificator = editedCompany.CompanyIdentificator, TypeOfAction = "Update" });
            }

            return View(editedCompany);
        }

        // GET: CompanyDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CompanyDetails(string companyIdentificator)
        {
            var company = _context.companyRepository.GetCompanyById(companyIdentificator);
            var usersConnectedToCompany = _context.userRepository.GetUsersConnectedToCompany(companyIdentificator);

            List<DisplayUserViewModel> listOfUsers = new List<DisplayUserViewModel>();

            if (usersConnectedToCompany.Count != 0)
            {
                listOfUsers = _mapper.Map<List<DisplayUserViewModel>>(usersConnectedToCompany);
                listOfUsers.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).ToList().Select(s => s.CompanyName).ToList());
                listOfUsers.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).ToList().Select(s => s.CompanyName).ToList());
                listOfUsers.ForEach(z => z.Roles = _context.userRepository.TranslateRoles(z.Roles));
            }

            CompanyDetailsViewModel companyDetails = _mapper.Map<CompanyDetailsViewModel>(company);
            companyDetails.UsersConnectedToCompany = listOfUsers;

            return View(companyDetails);
        }

        // GET: DeleteCompanyHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCompanyHub(string companyIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(companyIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteCompanyEntityLink(companyIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteCompany
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCompany(string companyIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(companyIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel companyToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = companyIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie przedsiębiorstwa"
                };

                return View("DeleteEntity", companyToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteCompany
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteCompany(DeleteEntityViewModel companyToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var company = _context.companyRepository.GetCompanyById(companyToDelete.EntityIdentificator);

            if (company == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, companyToDelete.Code))
            {
                var logInfoDeleteCompany = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteCompany"]);
                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateUsers"]);

                _context.companyRepository.DeleteCompany(companyToDelete.EntityIdentificator);
                _logger.AddCompanyLog(company, logInfoDeleteCompany);

                var updatedUsers = _context.userRepository.DeleteCompanyFromUsers(companyToDelete.EntityIdentificator);
                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);

                var logInfoPersonalDeleteCompany = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteCompany"], "Nazwa przedsiębiorstwa: " + company.CompanyName);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteCompany);

                var logInfoPersonalDeleteUserCompany = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserCompany"], "Nazwa przedsiębiorstwa: " + company.CompanyName);
                _context.personalLogRepository.AddPersonalUsersLogs(updatedUsers.Select(z=> z.Id).ToList(), logInfoPersonalDeleteUserCompany);

                return RedirectToAction("DisplayAllCertificates", "Certificates", new { message = "Usunięto wskazane przedsiębiorstwo" });
            }

            return View("DeleteEntity", companyToDelete);
        }


        // GET: WorkerCompanies
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerCompanies()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker);

            List<DisplayCompanyViewModel> companiesList = _mapper.Map<List<DisplayCompanyViewModel>>(companies); 

            return View(companiesList);
        }

        // GET: CompanyWorkers
        [Authorize(Roles = "Company")]
        public ActionResult CompanyWorkers()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(user.CompanyRoleManager.FirstOrDefault());
            var companyManagers = _context.userRepository.GetUsersManagersByCompanyId(user.CompanyRoleManager.FirstOrDefault());

            DisplayAllCompanyEmployees companyEmployees = new DisplayAllCompanyEmployees();
            companyEmployees.CompanyUserRoleWorker = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkers);
            companyEmployees.CompanyUserRoleManager = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyManagers);

            return View(companyEmployees);
        }

        // GET: CompanyInformation
        [Authorize(Roles = "Company")]
        public ActionResult CompanyInformation()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var company = _context.companyRepository.GetCompanyById(user.CompanyRoleManager.FirstOrDefault());

            DisplayCompanyViewModel companyInformation = _mapper.Map<DisplayCompanyViewModel>(company);

            return View(companyInformation);
        }

        // GET: CompanyWorkersCompetences
        [Authorize(Roles = "Company")]
        public ActionResult CompanyWorkersCompetences()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(user.CompanyRoleManager.FirstOrDefault());

            var companyWorkersGivenCertificates = companyWorkers.SelectMany(z => z.GivenCertificates).ToList();
            var companyWorkersCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(companyWorkersGivenCertificates).Select(z => z.Certificate).Distinct().ToList();

            var certificatesDetails = _context.certificateRepository.GetCertificatesById(companyWorkersCertificates);

            var companyWorkersGivenDegrees = companyWorkers.SelectMany(z => z.GivenDegrees).ToList();
            var companyWorkersDegrees = _context.givenDegreeRepository.GetGivenDegreesById(companyWorkersGivenDegrees).Select(z => z.Degree).Distinct().ToList();

            var degreesDetails = _context.degreeRepository.GetDegreesById(companyWorkersDegrees);

            List<DisplayDegreeViewModel> listOfDegrees = new List<DisplayDegreeViewModel>();

            foreach (var degree in degreesDetails)
            {
                var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);
                var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);

                DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);

                singleDegree.RequiredCertificates = requiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                singleDegree.RequiredDegrees = requiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

                listOfDegrees.Add(singleDegree);
            }

            DisplayCompanyWorkersCompetencesViewModel companyWorkersCompetences = new DisplayCompanyWorkersCompetencesViewModel();
            companyWorkersCompetences.Certificates = _mapper.Map<List<DisplayCertificateViewModel>>(certificatesDetails);
            companyWorkersCompetences.Certificates.ToList().ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

            companyWorkersCompetences.Degrees = listOfDegrees;

            return View(companyWorkersCompetences);
        }
    }
}

