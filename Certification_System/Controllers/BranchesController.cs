using Certification_System.DAL;
using Certification_System.Models;
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

        // GET: AddNewBranchConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewBranchConfirmation(string BranchName)
        {
            if (BranchName == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }
            return View(BranchName);
        }

        // GET: AddNewBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewBranch()
        {
            return View();
        }

        // POST: AddNewBranch
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewBranch(AddBranchViewModel newBranch)
        {
            if (ModelState.IsValid)
            {
                Branch branch = new Branch
                {
                    Name = newBranch.Name
                };

                return RedirectToAction("AddNewBranchConfirmation", new { BranchName = newBranch.Name });
            }

            return View(newBranch);
        }

    }
}