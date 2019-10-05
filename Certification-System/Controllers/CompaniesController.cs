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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCompanyLog(company, logInfo);

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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCompanyLog(company, logInfo);

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
                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.companyRepository.DeleteCompany(companyToDelete.EntityIdentificator);
                _logger.AddCompanyLog(company, logInfo);

                var updatedUsers = _context.userRepository.DeleteCompanyFromUsers(companyToDelete.EntityIdentificator);
                _logger.AddUsersLogs(updatedUsers, logInfoUpdate);

                return RedirectToAction("DisplayAllCertificates", "Certificates", new { message = "Usunięto wskazane przedsiębiorstwo" });
            }

            return View("DeleteEntity", companyToDelete);
        }


        // GET: DeleteCompany
        [Authorize(Roles = "Worker")]
        [HttpGet]
        public ActionResult WorkerCompanies()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker);

            List<DisplayCompanyViewModel> companiesList = _mapper.Map<List<DisplayCompanyViewModel>>(companies); 

            return View(companiesList);
        }
    }
}

