using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IBranchRepository
    {
        ICollection<Branch> GetListOfBranches();
        void UpdateBranch(Branch branch);
        Branch GetBranchById(string branchIdentificator);
        void AddBranch(Branch branch);
        ICollection<SelectListItem> GetBranchesAsSelectList();
        ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators);
        void DeleteBranch(string branchIdentificator);
    }
}
