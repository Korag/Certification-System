﻿using Certification_System.Models;
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
        ICollection<SelectListItem> GetRolesAsSelectList();

        #region Users
        ICollection<IdentityUser> GetUsers();
        IdentityUser GetUserById(string userIdentificator);
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
        ICollection<SelectListItem> GetCoursesAsSelectList();
        ICollection<Course> GetActiveCourses();
        #endregion

        #region Meeting
        ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators);
        void AddMeeting(Meeting meeting);
        #endregion

        #region Instructor
        ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId);
        Instructor GetInstructorById(string instructorIdentificator);
        void AddInstructor(Instructor instructor);
        ICollection<Instructor> GetInstructors();
        #endregion

        #region Company
        ICollection<Company> GetCompanies();
        ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators);
        void AddCompany(Company company);
        Company GetCompanyById(string companyIdentificator);
        ICollection<SelectListItem> GetCompaniesAsSelectList();
        #endregion
    }
}