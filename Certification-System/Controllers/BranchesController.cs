using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;

namespace Certification_System.Controllers
{
    public class BranchesController : Controller
    {
        private MongoOperations _context;
        private IMapper _mapper;
        private IKeyGenerator _keyGenerator;

        public BranchesController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: DisplayAllBranches
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllBranches()
        {
            var BranchesList = _context.branchRepository.GetBranches();
            List<DisplayBranchViewModel> existingBranches = _mapper.Map<List<DisplayBranchViewModel>>(BranchesList);

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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewBranch(AddBranchViewModel newBranch)
        {
            if (ModelState.IsValid)
            {
                Branch branch = _mapper.Map<Branch>(newBranch);
                branch.BranchIdentificator = _keyGenerator.GenerateNewId();

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
            EditBranchViewModel branchToUpdate = _mapper.Map<EditBranchViewModel>(Branch);

            return View(branchToUpdate);
        }

        // POST: EditBranch
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditBranch(EditBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var OriginBranch = _context.branchRepository.GetBranchById(editedBranch.BranchIdentificator);

                OriginBranch = _mapper.Map<EditBranchViewModel, Branch>(editedBranch, OriginBranch);
                _context.branchRepository.UpdateBranch(OriginBranch);

                return RedirectToAction("AddNewBranchConfirmation", "Branches" , new { BranchIdentificator = OriginBranch.BranchIdentificator, TypeOfAction = "Update" });
            }

            return View(editedBranch);
        }
    }
}