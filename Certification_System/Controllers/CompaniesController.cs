using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

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
        public ActionResult AddNewCompanyConfirmation(string companyName)
        {
            if (companyName != null)
            {
                Company company = _context.GetCompanyByName(companyName);

                AddCompanyViewModel addedCompany = new AddCompanyViewModel
                {
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
                    CompanyName = newCompany.CompanyName,
                    Email = newCompany.Email,
                    Phone = newCompany.Phone,
                    Country = newCompany.Country,
                    City = newCompany.City,
                    PostCode = newCompany.PostCode,
                    Address = newCompany.PostCode,
                    NumberOfApartment = newCompany.NumberOfApartment
                };

                _context.AddCompany(company);

                return RedirectToAction("AddNewCompanyConfirmation", new { companyName = newCompany.CompanyName});
            }

            return View(newCompany);
        }
    }
}