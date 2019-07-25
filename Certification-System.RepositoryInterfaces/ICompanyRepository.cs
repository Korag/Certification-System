using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ICompanyRepository
    {
        ICollection<Company> GetCompanies();
        ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators);
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        Company GetCompanyById(string companyIdentificator);
        ICollection<SelectListItem> GetCompaniesAsSelectList();
    }
}
