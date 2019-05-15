﻿using Certification_System.Models;
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
        ICollection<SelectListItem> GetUsersAsSelectList();
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
        ICollection<SelectListItem> GetCertificatesAsSelectList();
        #endregion

        #region Course
        void AddCourse(Course course);
        Course GetCourseById(string courseIdentificator);
        ICollection<SelectListItem> GetCoursesAsSelectList();
        ICollection<Course> GetActiveCourses();
        Course GetCourseByMeetingId(string meetingIdentificator);
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
    }
}