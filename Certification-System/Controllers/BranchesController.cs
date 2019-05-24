﻿using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        public IActionResult AddNewBranchConfirmation(string BranchIdentificator, string TypeOfAction)
        {
            ViewBag.TypeOfAction = TypeOfAction;

            var Branch = _context.GetBranchById(BranchIdentificator);

            if (BranchIdentificator == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }

            return View(Branch);
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
                    BranchIdentificator = ObjectId.GenerateNewId().ToString(),
                    Name = newBranch.Name
                };

                _context.AddBranch(branch);
                return RedirectToAction("AddNewBranchConfirmation", new { BranchIdentificator = newBranch.BranchIdentificator, TypeOfAction = "Add" });
            }

            return View(newBranch);
        }

        // GET: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditBranch(string BranchIdentificator)
        {
            var Branch = _context.GetBranchById(BranchIdentificator);

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
        public IActionResult EditBranch(AddBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var OriginBranch = _context.GetBranchById(editedBranch.BranchIdentificator);
                OriginBranch.Name = editedBranch.Name;

                _context.UpdateBranch(OriginBranch);

                return RedirectToAction("AddNewBranchConfirmation", "Branches" , new { BranchIdentificator = OriginBranch.BranchIdentificator, TypeOfAction = "Edit" });
            }

            return View(editedBranch);
        }
    }
}