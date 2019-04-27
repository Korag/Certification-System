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
        #region Users
        ICollection<IdentityUser> GetUsers();
        #endregion
    
        #region Branches
        ICollection<Branch> GetBranches();
        void AddBranch(Branch branch);
        ICollection<SelectListItem> GetBranchesAsSelectList();
        ICollection<string> GetBranchesById(ICollection<string> BranchesId);
        #endregion

        #region Certificate
        ICollection<Certificate> GetCertificates();
        void AddCertificate(Certificate certificate);
        Certificate GetCertificateByCertId(string certificateIdentificator);
        #endregion

        #region Course
        void AddCourse(Course course);
        Course GetCourseByCourId(string courseIdentificator);
        #endregion

        #region Meeting
        ICollection<Meeting> GetMeetingsById(ICollection<string> MeetingsId);
        #endregion

        #region Instructor
        ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId);
        #endregion

        #region Company
        ICollection<Company> GetCompanies();
        void AddCompany(Company company);
        Company GetCompanyByName(string companyName);
        #endregion
    }
}