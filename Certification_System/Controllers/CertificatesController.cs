using Certification_System.DAL;
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
                Branches = _context.GetBranches()
            };

            return View();
        }

        // POST: AddNewCertificate
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCertificate(AddCertificateToDbViewModel addedCertificate)
        {
            return View();
        }



    }
}