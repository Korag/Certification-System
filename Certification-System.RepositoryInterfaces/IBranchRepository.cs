using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IBranchRepository
    {
        ICollection<Branch> GetBranches();
        void UpdateBranch(Branch branch);
        Branch GetBranchById(string branchIdentificator);
        void AddBranch(Branch branch);
        ICollection<SelectListItem> GetBranchesAsSelectList();
        ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators);

    }
}
