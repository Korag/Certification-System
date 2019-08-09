using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IUserRepository
    {
        ICollection<CertificationPlatformUser> GetListOfUsers();
        ICollection<CertificationPlatformUser> GetListOfInstructors();
        ICollection<SelectListItem> GetRolesAsSelectList();
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
        CertificationPlatformUser GetInstructorById(string userIdentificator);
        ICollection<CertificationPlatformUser> GetInstructorsById(ICollection<string> userIdentificators);
        ICollection<SelectListItem> GetInstructorsAsSelectList();
    }
}
