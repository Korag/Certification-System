using Certification_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Certification_System.DAL
{
    public interface IDatabaseOperations
    {
        ICollection<SelectListItem> GetRolesAsSelectList();

        #region Users

        ICollection<CertificationPlatformUser> GetUsers();
        CertificationPlatformUser GetUserById(string userIdentificator);
        ICollection<CertificationPlatformUser> GetUsersById(ICollection<string> userIdentificators);
        ICollection<SelectListItem> GetUsersAsSelectList();
        void AddUserCertificate(string userIdentificator, string givenCertificateIdentificator);
        CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator);
        ICollection<CertificationPlatformUser> GetUsersByGivenCertificateId(ICollection<string> givenCertificatesIdentificators);

        #endregion

        #region Branches

        ICollection<Branch> GetBranches();
        void UpdateBranch(Branch branch);
        Branch GetBranchById(string branchIdentificator);
        void AddBranch(Branch branch);
        ICollection<SelectListItem> GetBranchesAsSelectList();
        ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators);

        #endregion

        #region Certificate

        ICollection<Certificate> GetCertificates();
        void AddCertificate(Certificate certificate);
        void UpdateCertificate(Certificate editedCertificate);
        Certificate GetCertificateById(string certificateIdentificator);
        ICollection<SelectListItem> GetCertificatesAsSelectList();
        ICollection<Certificate> GetCertificatesById(ICollection<string> certificateIdentificators);
        #endregion

        #region Course

        void AddCourse(Course course);
        Course GetCourseById(string courseIdentificator);
        ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators);
        ICollection<SelectListItem> GetCoursesAsSelectList();
        ICollection<Course> GetActiveCourses();
        Course GetCourseByMeetingId(string meetingIdentificator);
        ICollection<Course> GetCourses();

        #endregion

        #region Meeting

        ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators);
        Meeting GetMeetingById(string meetingsIdentificators);
        void AddMeeting(Meeting meeting);
        void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator);

        #endregion

        #region Instructor

        ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId);
        Instructor GetInstructorById(string instructorIdentificator);
        void AddInstructor(Instructor instructor);
        ICollection<Instructor> GetInstructors();
        ICollection<SelectListItem> GetInstructorsAsSelectList();

        #endregion

        #region Company

        ICollection<Company> GetCompanies();
        ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators);
        void AddCompany(Company company);
        Company GetCompanyById(string companyIdentificator);
        ICollection<SelectListItem> GetCompaniesAsSelectList();

        #endregion

        #region GivenCertificate
        void AddGivenCertificate(GivenCertificate givenCertificate);
        GivenCertificate GetGivenCertificateById(string givenCertificateIdentificator);
        ICollection<GivenCertificate> GetGivenCertificates();
        ICollection<GivenCertificate> GetGivenCertificatesByIdOfCertificate(string certificateIdentificator);

        #endregion

        #region Degree

        ICollection<Degree> GetDegrees();
        ICollection<SelectListItem> GetDegreesAsSelectList();
        void AddDegree(Degree degree);
        Degree GetDegreeById(string degreeIdentificator);
        ICollection<Degree> GetDegreesById(ICollection<string> degreeIdentificators);

        #endregion

    }

}