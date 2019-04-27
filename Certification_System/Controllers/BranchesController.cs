using Certification_System.DAL;
using Certification_System.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Certification_System.Controllers
{
    public class BranchesController : Controller
    {
        private IDatabaseOperations _context;

        public BranchesController()
        {
            _context = new MongoOperations();
        }

        // GET: DisplayAllBranches
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllBranches()
        {
            var BranchesList = _context.GetBranches();

            List<AddBranchViewModel> existingBranches = new List<AddBranchViewModel>();

            foreach (var branch in BranchesList)
            {
                existingBranches.Add(new AddBranchViewModel
                {
                    Name = branch.Name
                });
            }

            return View(existingBranches);
        }

        // GET: AddNewBranch
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        // POST: AddNewBranch
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewBranch()
        {
            return View();
        }

    }
}