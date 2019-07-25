using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using Certification_System.Repository.DAL;

namespace Certification_System.Controllers
{
    public class BranchesController : Controller
    {
        private MongoOperations _context;

        public BranchesController(MongoOperations context)
        {
            _context = context;
        }

        // GET: DisplayAllBranches
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllBranches()
        {
            var BranchesList = _context.branchRepository.GetBranches();

            List<DisplayBranchViewModel> existingBranches = new List<DisplayBranchViewModel>();

            foreach (var branch in BranchesList)
            {
                existingBranches.Add(new DisplayBranchViewModel
                {
                    BranchIdentificator = branch.BranchIdentificator,
                    Name = branch.Name
                });
            }

            return View(existingBranches);
        }

        // GET: AddNewBranchConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewBranchConfirmation(string BranchIdentificator, string TypeOfAction)
        {
            ViewBag.TypeOfAction = TypeOfAction;

            var Branch = _context.branchRepository.GetBranchById(BranchIdentificator);

            if (BranchIdentificator == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }

            return View(Branch);
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
                    BranchIdentificator = ObjectId.GenerateNewId().ToString(),
                    Name = newBranch.Name
                };

                _context.branchRepository.AddBranch(branch);
                return RedirectToAction("AddNewBranchConfirmation", "Branches", new { BranchIdentificator = branch.BranchIdentificator, TypeOfAction = "Add" });
            }

            return View(newBranch);
        }

        // GET: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditBranch(string BranchIdentificator)
        {
            var Branch = _context.branchRepository.GetBranchById(BranchIdentificator);

            AddBranchViewModel branchToUpdate = new AddBranchViewModel
            {
                BranchIdentificator = Branch.BranchIdentificator,
                Name = Branch.Name
            };

            return View(branchToUpdate);
        }

        // POST: EditBranch
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditBranch(AddBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var OriginBranch = _context.branchRepository.GetBranchById(editedBranch.BranchIdentificator);
                OriginBranch.Name = editedBranch.Name;

                _context.branchRepository.UpdateBranch(OriginBranch);

                return RedirectToAction("AddNewBranchConfirmation", "Branches" , new { BranchIdentificator = OriginBranch.BranchIdentificator, TypeOfAction = "Update" });
            }

            return View(editedBranch);
        }
    }
}