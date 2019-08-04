using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly MongoContext _context;

        private readonly string _companiesCollectionName = "Companies";
        private IMongoCollection<Company> _companies;

        public CompanyRepository(MongoContext context)
        {
            _context = context;
        }

        public ICollection<SelectListItem> GetCompaniesAsSelectList()
        {
            var Companies = GetCompanies();
            List<SelectListItem> SelectList = new List<SelectListItem>();
            //SelectList.Add(new SelectListItem { Text = "---", Value = null });

            foreach (var company in Companies)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = company.CompanyName,
                            Value = company.CompanyIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<Company> GetCompanies()
        {
            _companies = _context.db.GetCollection<Company>(_companiesCollectionName);

            return _companies.AsQueryable().ToList();
        }

        public void AddCompany(Company company)
        {
            _companies = _context.db.GetCollection<Company>(_companiesCollectionName);
            _companies.InsertOne(company);
        }

        public void UpdateCompany(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, company.CompanyIdentificator);
            var result = _context.db.GetCollection<Company>(_companiesCollectionName).ReplaceOne(filter, company);
        }

        public Company GetCompanyById(string companyIdentificator)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, companyIdentificator);
            Company company = _context.db.GetCollection<Company>(_companiesCollectionName).Find<Company>(filter).FirstOrDefault();

            return company;
        }

        public ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators)
        {
            List<Company> Companies = new List<Company>();

            if (companyIdentificators != null)
            {
                foreach (var companyIdentificator in companyIdentificators)
                {
                    var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, companyIdentificator);
                    Company company = _context.db.GetCollection<Company>(_companiesCollectionName).Find<Company>(filter).FirstOrDefault();

                    Companies.Add(company);
                }

            }

            return Companies;
        }
    }
}
