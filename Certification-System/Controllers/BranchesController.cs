using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public IActionResult DisplayAllBranches()
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
        public IActionResult AddNewBranchConfirmation(string BranchName)
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
        public IActionResult AddNewBranch()
        {
            return View();
        }

        // POST: AddNewBranch
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewBranch(AddBranchViewModel newBranch)
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