using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public CertificatesController()
        {
            _context = new MongoOperations();
        }

        // GET: AddNewCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCertificate()
        {
            AddCertificateToDbViewModel newCertificate = new AddCertificateToDbViewModel
            {
                AvailableBranches = new List<SelectListItem>(),
                SelectedBranches = new List<string>()
            };

            newCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();

            return View(newCertificate);
        }

        // GET: AddNewCertificateConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCertificateConfirmation(string certificateIdentificator)
        {
            if (certificateIdentificator != null)
            {
                var Certificate = _context.GetCertificateById(certificateIdentificator);

                AddCertificateToDbViewModel addedCertificate = new AddCertificateToDbViewModel
                {
                    CertificateIndexer = Certificate.CertificateIndexer,
                    Name = Certificate.Name,
                    Description = Certificate.Description,
                };

                var BranchNames = _context.GetBranchesById(Certificate.Branches);

                addedCertificate.SelectedBranches = BranchNames;

                return View(addedCertificate);
            }
            return RedirectToAction(nameof(AddNewCertificate));
        }

        // POST: AddNewCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCertificate(AddCertificateToDbViewModel newCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = new Certificate
                {
                    CertificateIdentificator = ObjectId.GenerateNewId().ToString(),

                    CertificateIndexer = newCertificate.CertificateIndexer,
                    Name = newCertificate.Name, 
                    Description = newCertificate.Description,

                    Branches = newCertificate.SelectedBranches
                };

                _context.AddCertificate(certificate);

                return RedirectToAction("AddNewCertificateConfirmation", new { certificateIdentificator = certificate.CertificateIdentificator });
            }

            newCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (newCertificate.SelectedBranches == null)
            {
                newCertificate.SelectedBranches = new List<string>();
            }
            return View(newCertificate);
        }



    }
}