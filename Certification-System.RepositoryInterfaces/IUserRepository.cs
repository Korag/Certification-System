using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IUserRepository
    {
        ICollection<CertificationPlatformUser> GetListOfUsers();
        ICollection<CertificationPlatformUser> GetListOfInstructors();
        ICollection<CertificationPlatformUser> GetListOfWorkers();
        ICollection<CertificationPlatformUser> GetListOfAdmins();
        ICollection<SelectListItem> GetRolesAsSelectList();
        ICollection<SelectListItem> GetAvailableRoleFiltersAsSelectList();
        void UpdateUser(CertificationPlatformUser user);
        CertificationPlatformUser GetUserById(string userIdentificator);
        ICollection<CertificationPlatformUser> GetUsersById(ICollection<string> userIdentificators);
        ICollection<SelectListItem> GetUsersAsSelectList();
        void AddUserCertificate(string userIdentificator, string givenCertificateIdentificator);
        CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator);
        ICollection<CertificationPlatformUser> GetUsersByGivenCertificatesId(ICollection<string> givenCertificatesIdentificators);
        ICollection<SelectListItem> GetExaminersAsSelectList();
        ICollection<CertificationPlatformUser> GetUsersConnectedToCompany(string companyIdentificator);
        ICollection<CertificationPlatformUser> GetUsersByDegreeId(ICollection<string> degreeIdentificators);
        CertificationPlatformUser GetUserByGivenDegreeId(string givenDegreeIdentificator);
        void AddUserDegree(string userIdentificator, string givenDegreeIdentificator);
        CertificationPlatformUser GetInstructorById(string userIdentificator);
        ICollection<CertificationPlatformUser> GetInstructorsById(ICollection<string> userIdentificators);
        ICollection<SelectListItem> GetInstructorsAsSelectList();
        ICollection<SelectListItem> GetWorkersAsSelectList();
        ICollection<CertificationPlatformUser> AddUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators);
        CertificationPlatformUser AddUserToCourse(string courseIdentificator, string userIdentificator);
        void DeleteCourseFromUsersCollection(string courseIdentificator, ICollection<string> usersIdentificators);
        ICollection<string> TranslateRoles(ICollection<string> userRoles);
        CertificationPlatformUser GetUserByEmail(string emailAddress);
        ICollection<SelectListItem> GetAvailableCourseRoleFiltersAsSelectList();
        ICollection<CertificationPlatformUser> DeleteCompanyFromUsers(string companyIdentificator);
        CertificationPlatformUser DeleteUserGivenDegree(string givenDegreeIdentificator);
        CertificationPlatformUser DeleteUserGivenCertificate(string givenCertificateIdentificator);
        ICollection<CertificationPlatformUser> DeleteCourseFromUsers(string courseIdentificator);
        void DeleteUser(string userIdentificator);
        List<SelectListItem> GenerateSelectList(ICollection<string> usersIdentificators);
        ICollection<CertificationPlatformUser> GetUsersByGivenDegreesId(ICollection<string> givenDegreesIdentificators);
    }
}
