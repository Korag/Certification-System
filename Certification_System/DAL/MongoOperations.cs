using Certification_System.Models;
using MongoDB.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Certification_System.DAL
{
    public class MongoOperations : IDatabaseOperations
    {
        private MongoContext _context;

        private string _usersCollectionName = "AspNetUsers";
        private string _branchCollectionName = "Branches";

        //Collections
        private IMongoCollection<IdentityUser> _users;
        private IMongoCollection<Branch> _branches;

        public MongoOperations()
        {
            _context = new MongoContext();
        }

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

        #region Methods
        public ICollection<IdentityUser> GetUsers()
        {
            _users = _context.db.GetCollection<IdentityUser>(_usersCollectionName);
            return _users.AsQueryable().ToList();
        }
        #endregion
    }
}