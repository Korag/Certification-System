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

        private IMongoCollection<Branch> GetBranches()
        {
            return _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
        }

        public ICollection<Branch> GetListOfBranches()
        {
            return GetBranches().AsQueryable().ToList();
        }

        public void AddBranch(Branch branch)
        {
            GetBranches().InsertOne(branch);
        }

        public void UpdateBranch(Branch branch)
        {
            var filter = Builders<Branch>.Filter.Eq(x => x.BranchIdentificator, branch.BranchIdentificator);
            var result = GetBranches().ReplaceOne(filter, branch);
        }

        public ICollection<SelectListItem> GetBranchesAsSelectList()
        {
            GetBranches();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var branch in _branches.AsQueryable().ToList())
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
            var resultBranch = GetBranches().Find<Branch>(filter).FirstOrDefault();

            return resultBranch;
        }

        public ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators)
        {
            var filter = Builders<Branch>.Filter.Where(z => branchesIdentificators.Contains(z.BranchIdentificator));
            var result = GetBranches().Find<Branch>(filter).Project(z => z.Name).ToList();

            return result;
        }
    }
}
