using Certification_System.Models;
using MongoDB.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.DAL
{
    public interface IDatabaseOperations
    {
        #region Users
        ICollection<IdentityUser> GetUsers();
        #endregion
    
        #region Branches
        ICollection<Branch> GetBranches();
        void AddBranch(Branch branch);
        ICollection<SelectListItem> GetBranchesAsSelectList();
        ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators);
        #endregion

        #region Certificate
        ICollection<Certificate> GetCertificates();
        void AddCertificate(Certificate certificate);
        Certificate GetCertificateById(string certificateIdentificator);
        #endregion

        #region Course
        void AddCourse(Course course);
        Course GetCourseById(string courseIdentificator);
        #endregion

        #region Meeting
        ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators);
        #endregion

        #region Instructor
        ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId);
        Instructor GetInstructorById(string instructorIdentificator);
        void AddInstructor(Instructor instructor);
        ICollection<Instructor> GetInstructors();
        #endregion

        #region Company
        ICollection<Company> GetCompanies();
        void AddCompany(Company company);
        Company GetCompanyById(string companyIdentificator);
        #endregion
    }
}