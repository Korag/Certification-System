using Certification_System.DAL;
using Certification_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}