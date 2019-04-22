using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
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
            AddCertificateToDbViewModel addedCertificate = new AddCertificateToDbViewModel
            {
                AvailableBranches = new List<SelectListItem>(),
                SelectedBranches = new List<string>()
            };

            addedCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();

            return View(addedCertificate);
        }

        // GET: AddNewCertificateConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCertificateConfirmation(string CertificateIdentificator)
        {
            if (CertificateIdentificator != null)
            {
                var Certificate = _context.GetCertificateByCertId(CertificateIdentificator);
                return View(Certificate);
            }
            return RedirectToAction(nameof(AddNewCertificate));
        }

        // POST: AddNewCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCertificate(AddCertificateToDbViewModel addedCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = new Certificate
                {
                    Name = addedCertificate.Name,
                    CertificateIdentificator = addedCertificate.CertificateIdentificator,
                    Description = addedCertificate.Description,

                    Branches = addedCertificate.SelectedBranches
                };

                _context.AddCertificate(certificate);

                return RedirectToAction("AddNewCertificateConfirmation", new { CertificateIdentificator = addedCertificate.CertificateIdentificator });
            }

            addedCertificate.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (addedCertificate.SelectedBranches == null)
            {
                addedCertificate.SelectedBranches = new List<string>();
            }
            return View(addedCertificate);
        }



    }
}