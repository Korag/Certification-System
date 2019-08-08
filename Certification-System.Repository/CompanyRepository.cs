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

        private IMongoCollection<Company> GetCompanies()
        {
            return _companies = _context.db.GetCollection<Company>(_companiesCollectionName);
        }

        public ICollection<Company> GetListOfCompanies()
        {
            return GetCompanies().AsQueryable().ToList();
        }
        public ICollection<SelectListItem> GetCompaniesAsSelectList()
        {
            GetCompanies();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var company in _companies.AsQueryable().ToList())
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

        public void AddCompany(Company company)
        {
            GetCompanies().InsertOne(company);
        }

        public void UpdateCompany(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, company.CompanyIdentificator);
            var result = GetCompanies().ReplaceOne(filter, company);
        }

        public Company GetCompanyById(string companyIdentificator)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, companyIdentificator);
            var resultCompany = GetCompanies().Find<Company>(filter).FirstOrDefault();

            return resultCompany;
        }

        public ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators)
        {
            var filter = Builders<Company>.Filter.Where(z => companyIdentificators.Contains(z.CompanyIdentificator));
            var result = GetCompanies().Find<Company>(filter).ToList();

            return result;
        }
    }
}
