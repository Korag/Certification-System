using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IUserRepository
    {
        ICollection<SelectListItem> GetRolesAsSelectList();

        ICollection<CertificationPlatformUser> GetUsers();
        void UpdateUser(CertificationPlatformUser user);
        CertificationPlatformUser GetUserById(string userIdentificator);
        ICollection<CertificationPlatformUser> GetUsersById(ICollection<string> userIdentificators);
        ICollection<SelectListItem> GetUsersAsSelectList();
        void AddUserCertificate(string userIdentificator, string givenCertificateIdentificator);
        CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator);
        ICollection<CertificationPlatformUser> GetUsersByGivenCertificateId(ICollection<string> givenCertificatesIdentificators);
        ICollection<CertificationPlatformUser> GetUsersConnectedToCompany(string companyIdentificator);
        ICollection<CertificationPlatformUser> GetUsersByDegreeId(ICollection<string> degreeIdentificators);
        CertificationPlatformUser GetUserByGivenDegreeId(string givenDegreeIdentificator);
        void AddUserDegree(string userIdentificator, string givenDegreeIdentificator);
    }
}
