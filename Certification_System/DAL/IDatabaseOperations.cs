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
        #endregion

        #region Certificate
        ICollection<Certificate> GetCertificates();
        void AddCertificate(Certificate certificate);
        Certificate GetCertificateByCertId(string certificateIdentificator);
        #endregion

    }
}