using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;

namespace Certification_System.Controllers
{
    public class BranchesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

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

        // GET: ConfirmationOfActionOnBranch
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnBranch(string branchIdentificator, string typeOfAction)
        {
            ViewBag.TypeOfAction = typeOfAction;

            var Branch = _context.branchRepository.GetBranchById(branchIdentificator);

            if (branchIdentificator == null)
            {
                return RedirectToAction(nameof(AddNewBranch));
            }

            DisplayBranchViewModel modifiedBranch = _mapper.Map<DisplayBranchViewModel>(Branch);

            return View(modifiedBranch);
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

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches", new { branchIdentificator = branch.BranchIdentificator, typeOfAction = "Add" });
            }

            return View(newBranch);
        }

        // GET: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditBranch(string branchIdentificator)
        {
            var Branch = _context.branchRepository.GetBranchById(branchIdentificator);
            EditBranchViewModel branchToUpdate = _mapper.Map<EditBranchViewModel>(Branch);

            return View(branchToUpdate);
        }

        // POST: EditBranch
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditBranch(EditBranchViewModel editedBranch)
        {
            if (ModelState.IsValid)
            {
                var OriginBranch = _context.branchRepository.GetBranchById(editedBranch.BranchIdentificator);

                OriginBranch = _mapper.Map<EditBranchViewModel, Branch>(editedBranch, OriginBranch);

                _context.branchRepository.UpdateBranch(OriginBranch);

                return RedirectToAction("ConfirmationOfActionOnBranch", "Branches" , new { branchIdentificator = OriginBranch.BranchIdentificator, typeOfAction = "Update" });
            }

            return View(editedBranch);
        }
    }
}