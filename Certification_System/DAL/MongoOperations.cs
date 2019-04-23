using Certification_System.Models;
using MongoDB.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.DAL
{
    public class MongoOperations : IDatabaseOperations
    {
        private MongoContext _context;

        private string _usersCollectionName = "AspNetUsers";
        private string _branchCollectionName = "Branches";
        private string _certificatesCollectionName = "Certificates";

        //Collections
        private IMongoCollection<IdentityUser> _users;
        private IMongoCollection<Branch> _branches;
        private IMongoCollection<Certificate> _certificates;

        public MongoOperations()
        {
            _context = new MongoContext();
        }

        #region Methods
        public void AddBranch(Branch branch)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            _branches.InsertOne(branch);
        }

        public ICollection<Branch> GetBranches()
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            return _branches.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetBranchesAsSelectList()
        {
            var Branches = GetBranches();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var branch in Branches)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = branch.Name,
                            Value = branch.Id
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<string> GetBranchesById(ICollection<string> BranchesId)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            List<string> BranchesNames = new List<string>();

            foreach (var branch in BranchesId)
            {
                var filter = Builders<Branch>.Filter.Eq(x => x.Id, branch);
                BranchesNames.Add(_branches.Find<Branch>(filter).Project(z=> z.Name).FirstOrDefault());
            }
           
            return BranchesNames.AsQueryable().ToList();
        }

        public ICollection<Certificate> GetCertificates()
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            return _certificates.AsQueryable().ToList();
        }


        public void AddCertificate(Certificate certificate)
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            _certificates.InsertOne(certificate);
        }

        public Certificate GetCertificateByCertId(string certificateIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, certificateIdentificator);
            Certificate certificate = _context.db.GetCollection<Certificate>(_certificatesCollectionName).Find<Certificate>(filter).FirstOrDefault();
            return certificate;
        }

        public ICollection<IdentityUser> GetUsers()
        {
            _users = _context.db.GetCollection<IdentityUser>(_usersCollectionName);
            return _users.AsQueryable().ToList();
        }
        #endregion
    }
}