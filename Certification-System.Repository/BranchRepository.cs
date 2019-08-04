using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class BranchRepository : IBranchRepository
    {
        private readonly MongoContext _context;

        private readonly string _branchCollectionName = "Branches";
        private IMongoCollection<Branch> _branches;

        public BranchRepository(MongoContext context)
        {
            _context = context;
        }

        public void AddBranch(Branch branch)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            _branches.InsertOne(branch);
        }

        public void UpdateBranch(Branch branch)
        {
            var filter = Builders<Branch>.Filter.Eq(x => x.BranchIdentificator, branch.BranchIdentificator);
            var result = _context.db.GetCollection<Branch>(_branchCollectionName).ReplaceOne(filter, branch);
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
                            Value = branch.BranchIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public Branch GetBranchById(string branchIdentificator)
        {
            var filter = Builders<Branch>.Filter.Eq(x => x.BranchIdentificator, branchIdentificator);
            Branch branch = _context.db.GetCollection<Branch>(_branchCollectionName).Find<Branch>(filter).FirstOrDefault();

            return branch;
        }

        public ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            List<string> BranchesNames = new List<string>();

            foreach (var branch in branchesIdentificators)
            {
                var filter = Builders<Branch>.Filter.Eq(x => x.BranchIdentificator, branch);
                BranchesNames.Add(_branches.Find<Branch>(filter).Project(z => z.Name).FirstOrDefault());
            }

            return BranchesNames.AsQueryable().ToList();
        }
    }
}
