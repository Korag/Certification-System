using Certification_System.DAL;
using Certification_System.Entitities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Certification_System.Controllers
{
    public class CompaniesController : Controller
    {
        private IDatabaseOperations _context;

        public CompaniesController()
        {
            _context = new MongoOperations();
        }

        // GET: DisplayAllCompanies
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCompanies()
        {
            var Companies = _context.GetCompanies();
            List<AddCompanyViewModel> DisplayCompanies = new List<AddCompanyViewModel>();

            foreach (var company in Companies)
            {
                AddCompanyViewModel singleCompany = new AddCompanyViewModel
                {
                    CompanyIdentificator = company.CompanyIdentificator,
                    CompanyName = company.CompanyName,
                    Email = company.Email,
                    Phone = company.Phone,
                    Country = company.Country,
                    City = company.City,
                    PostCode = company.PostCode,
                    Address = company.Address,
                    NumberOfApartment = company.NumberOfApartment
                };

                DisplayCompanies.Add(singleCompany);
            }

            return View(DisplayCompanies);
        }


        // GET: AddNewCompany
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCompany()
        {
            return View();
        }

        // GET: AddNewCompanyConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCompanyConfirmation(string companyIdentificator, string TypeOfAction)
        {
            if (companyIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                Company company = _context.GetCompanyById(companyIdentificator);

                AddCompanyViewModel addedCompany = new AddCompanyViewModel
                {
                    CompanyIdentificator = company.CompanyIdentificator,

                    CompanyName = company.CompanyName,
                    Email = company.Email,
                    Phone = company.Phone,
                    Country = company.Country,
                    City = company.City,
                    PostCode = company.PostCode,
                    Address = company.PostCode,
                    NumberOfApartment = company.NumberOfApartment
                };

                return View(addedCompany);
            }

            return RedirectToAction(nameof(AddNewCompany));
        }

        // GET: AddNewCompany
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCompany(AddCompanyViewModel newCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = new Company
                {
                    CompanyIdentificator = ObjectId.GenerateNewId().ToString(),

                    CompanyName = newCompany.CompanyName,
                    Email = newCompany.Email,
                    Phone = newCompany.Phone,
                    Country = newCompany.Country,
                    City = newCompany.City,
                    PostCode = newCompany.PostCode,
                    Address = newCompany.Address,
                    NumberOfApartment = newCompany.NumberOfApartment
                };

                _context.AddCompany(company);

                return RedirectToAction("AddNewCompanyConfirmation", new { companyIdentificator = company.CompanyIdentificator, TypeOfAction = "Add" });
            }

            return View(newCompany);
        }

        // GET: EditCompany
        [Authorize(Roles = "Admin")]
        public ActionResult EditCompany(string companyIdentificator)
        {
            var Company = _context.GetCompanyById(companyIdentificator);

            AddCompanyViewModel companyToUpdate = new AddCompanyViewModel
            {
                CompanyIdentificator = Company.CompanyIdentificator,

                CompanyName = Company.CompanyName,
                Email = Company.Email,
                Phone = Company.Phone,
                Country = Company.Country,
                City = Company.City,
                PostCode = Company.PostCode,
                Address = Company.Address,
                NumberOfApartment = Company.NumberOfApartment
            };

            return View(companyToUpdate);
        }

        // POST: EditCompany
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCompany(AddCompanyViewModel editedCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = new Company
                {
                    CompanyIdentificator = editedCompany.CompanyIdentificator,
                    CompanyName = editedCompany.CompanyName,
                    Email = editedCompany.Email,
                    Phone = editedCompany.Phone,
                    Country = editedCompany.Country,
                    City = editedCompany.City,
                    PostCode = editedCompany.PostCode,
                    Address = editedCompany.Address,
                    NumberOfApartment = editedCompany.NumberOfApartment
                };

                _context.UpdateCompany(company);

                return RedirectToAction("AddNewCompanyConfirmation", "Companies", new { companyIdentificator = editedCompany.CompanyIdentificator, TypeOfAction = "Update" });
            }

            return View(editedCompany);
        }

        // GET: CertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CompanyDetails(string companyIdentificator)
        {
            var Company = _context.GetCompanyById(companyIdentificator);

            var UsersConnectedToCompany = _context.GetUsersConnectedToCompany(companyIdentificator);

            List<DisplayUsersViewModel> ListOfUsers = new List<DisplayUsersViewModel>();

            if (UsersConnectedToCompany.Count != 0)
            {
                foreach (var user in UsersConnectedToCompany)
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

            CompanyDetailsViewModel companyDetails = new CompanyDetailsViewModel
            {
                CompanyIdentificator = Company.CompanyIdentificator,
                CompanyName = Company.CompanyName,
                Email = Company.Email,
                Phone = Company.Phone,
                Country = Company.Country,
                City = Company.City,
                PostCode = Company.PostCode,
                Address = Company.Address,
                NumberOfApartment = Company.NumberOfApartment,

                UsersConnectedToCompany = ListOfUsers
            };

            return View(companyDetails);
        }
    }
}

