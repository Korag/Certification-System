using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using System.Linq;
using Certification_System.Services;

namespace Certification_System.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public CompaniesController(
            MongoOperations context, 
            IMapper mapper, 
            IKeyGenerator keyGenerator,
            ILogService logger)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
        }

        // GET: DisplayAllCompanies
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCompanies()
        {
            var Companies = _context.companyRepository.GetListOfCompanies();
            List<DisplayCompanyViewModel> DisplayCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            return View(DisplayCompanies);
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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCompanyLog(company, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCompany", new { companyIdentificator = company.CompanyIdentificator, TypeOfAction = "Add" });
            }

            return View(newCompany);
        }

        // GET: EditCompany
        [Authorize(Roles = "Admin")]
        public ActionResult EditCompany(string companyIdentificator)
        {
            var Company = _context.companyRepository.GetCompanyById(companyIdentificator);
            EditCompanyViewModel companyToUpdate = _mapper.Map<EditCompanyViewModel>(Company);

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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCompanyLog(company, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCompany", "Companies", new { companyIdentificator = editedCompany.CompanyIdentificator, TypeOfAction = "Update" });
            }

            return View(editedCompany);
        }

        // GET: CompanyDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CompanyDetails(string companyIdentificator)
        {
            var Company = _context.companyRepository.GetCompanyById(companyIdentificator);
            var UsersConnectedToCompany = _context.userRepository.GetUsersConnectedToCompany(companyIdentificator);

            List<DisplayUserViewModel> ListOfUsers = new List<DisplayUserViewModel>();

            if (UsersConnectedToCompany.Count != 0)
            {
                ListOfUsers = _mapper.Map<List<DisplayUserViewModel>>(UsersConnectedToCompany);
                ListOfUsers.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).ToList().Select(s => s.CompanyName).ToList());
                ListOfUsers.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).ToList().Select(s => s.CompanyName).ToList());
            }

            CompanyDetailsViewModel companyDetails = _mapper.Map<CompanyDetailsViewModel>(Company);
            companyDetails.UsersConnectedToCompany = ListOfUsers;

            return View(companyDetails);
        }
    }
}

