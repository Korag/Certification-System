using Certification_System.DAL;
using Certification_System.DTOViewModels;
using Certification_System.Entitities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;


namespace Certification_System.Controllers
{
    public class DegreesController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public DegreesController()
        {
            _context = new MongoOperations();
        }

        // GET: AddNewDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewDegree()
        {
            AddDegreeViewModel newDegree = new AddDegreeViewModel
            {
                AvailableBranches = _context.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.GetDegreesAsSelectList().ToList(),

                SelectedBranches = new List<string>()
            };

            return View(newDegree);
        }

        // POST: AddNewDegree
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewDegree(AddDegreeViewModel newDegree)
        {
            if (ModelState.IsValid)
            {
                Degree degree = new Degree
                {
                    DegreeIdentificator = ObjectId.GenerateNewId().ToString(),

                    DegreeIndexer = newDegree.DegreeIndexer,
                    Name = newDegree.Name,
                    Description = newDegree.Description,

                    RequiredCertificates = newDegree.SelectedCertificates,
                    RequiredDegrees = newDegree.SelectedDegrees,
                    Branches = newDegree.SelectedBranches
                };

                if (degree.RequiredDegrees == null)
                {
                    degree.RequiredDegrees = new List<string>();
                }
                if (degree.RequiredCertificates == null)
                {
                    degree.RequiredCertificates = new List<string>();
                }

                _context.AddDegree(degree);

                return RedirectToAction("AddNewDegreeConfirmation", new { degreeIdentificator = degree.DegreeIdentificator, TypeOfAction = "Add" });
            }

            newDegree.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (newDegree.SelectedBranches == null)
            {
                newDegree.SelectedBranches = new List<string>();
            }
            return View(newDegree);
        }

        // GET: AddNewDegreeConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewDegreeConfirmation(string degreeIdentificator, string TypeOfAction)
        {
            if (degreeIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Degree = _context.GetDegreeById(degreeIdentificator);

                var RequiredCertificates = _context.GetCertificatesById(Degree.RequiredCertificates);
                var RequiredDegrees = _context.GetDegreesById(Degree.RequiredDegrees);
                var Branches = _context.GetBranchesById(Degree.Branches);

                DisplayDegreeViewModel addedDegree = new DisplayDegreeViewModel
                {
                    DegreeIdentificator = Degree.DegreeIdentificator,

                    DegreeIndexer = Degree.DegreeIndexer,
                    Name = Degree.Name,
                    Description = Degree.Description,

                    RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList(),
                    RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIdentificator + " " + z.Name).ToList(),
                    Branches = Branches
                };

                return View(addedDegree);
            }
            return RedirectToAction(nameof(AddNewDegree));
        }

        // GET: DisplayAllDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllDegrees()
        {
            var Degrees = _context.GetDegrees();
            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            foreach (var degree in Degrees)
            {
                var RequiredCertificates = _context.GetCertificatesById(degree.RequiredCertificates);
                var RequiredDegrees = _context.GetDegreesById(degree.RequiredDegrees);

                DisplayDegreeViewModel singleDegree = new DisplayDegreeViewModel
                {
                    DegreeIdentificator = degree.DegreeIdentificator,

                    DegreeIndexer = degree.DegreeIndexer,
                    Name = degree.Name,
                    Description = degree.Description,

                    RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList(),
                    RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList(),
                    Branches = _context.GetBranchesById(degree.Branches)
                };

                ListOfDegrees.Add(singleDegree);
            }

            return View(ListOfDegrees);
        }

        // GET: EditDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditDegree(string degreeIdentificator)
        {
            var Degree = _context.GetDegreeById(degreeIdentificator);

            AddDegreeViewModel degreeViewModel = new AddDegreeViewModel
            {
                DegreeIdentificator = Degree.DegreeIdentificator,
                DegreeIndexer = Degree.DegreeIndexer,
                Name = Degree.Name,
                Description = Degree.Description,
                SelectedBranches = Degree.Branches,
                SelectedCertificates = Degree.RequiredCertificates,
                SelectedDegrees = Degree.RequiredDegrees,

                AvailableBranches = _context.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.GetDegreesAsSelectList().ToList(),
            };

            return View(degreeViewModel);
        }

        // POST: EditDegree
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditDegree(AddDegreeViewModel editedDegree)
        {
            if (ModelState.IsValid)
            {
                var OriginDegree = _context.GetDegreeById(editedDegree.DegreeIdentificator);

                OriginDegree.DegreeIndexer = editedDegree.DegreeIndexer;
                OriginDegree.Name = editedDegree.Name;
                OriginDegree.Description = editedDegree.Description;
                OriginDegree.RequiredCertificates = editedDegree.SelectedCertificates;
                OriginDegree.RequiredDegrees = editedDegree.SelectedDegrees;
                OriginDegree.Branches = editedDegree.SelectedBranches;

                _context.UpdateDegree(OriginDegree);

                return RedirectToAction("AddNewDegreeConfirmation", "Degrees", new { degreeIdentificator = editedDegree.DegreeIdentificator, TypeOfAction = "Update" });
            }

            editedDegree.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            editedDegree.AvailableCertificates = _context.GetCertificatesAsSelectList().ToList();
            editedDegree.AvailableDegrees = _context.GetDegreesAsSelectList().ToList();

            return View(editedDegree);
        }

        // GET: DegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult DegreeDetails(string degreeIdentificator)
        {
            var Degree = _context.GetDegreeById(degreeIdentificator);

            var RequiredDegrees = _context.GetDegreesById(Degree.RequiredDegrees);
            var RequiredCertificates = _context.GetCertificatesById(Degree.RequiredCertificates);

            var GivenDegreesInstances = _context.GetGivenDegreesByIdOfDegree(degreeIdentificator);
            var GivenDegreesIdentificators = GivenDegreesInstances.Select(z => z.GivenDegreeIdentificator);
            var UsersWithDegree = _context.GetUsersByGivenDegreeId(GivenDegreesIdentificators.ToList());

            List<DisplayListOfCertificatesViewModel> ListOfCertificates = new List<DisplayListOfCertificatesViewModel>();

            if (RequiredCertificates.Count != 0)
            {
                foreach (var certificate in RequiredCertificates)
                {
                    DisplayListOfCertificatesViewModel singleCertificate = new DisplayListOfCertificatesViewModel
                    {
                        CertificateIdentificator = certificate.CertificateIdentificator,

                        CertificateIndexer = certificate.CertificateIndexer,
                        Name = certificate.Name,
                        Description = certificate.Description,
                        Branches = _context.GetBranchesById(certificate.Branches),
                        Depreciated = certificate.Depreciated
                    };

                    ListOfCertificates.Add(singleCertificate);
                }
            }

            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            if (RequiredDegrees.Count != 0)
            {
                foreach (var degree in RequiredDegrees)
                {
                    DisplayDegreeViewModel singleDegree = new DisplayDegreeViewModel
                    {
                        DegreeIdentificator = degree.DegreeIdentificator,

                        DegreeIndexer = degree.DegreeIndexer,
                        Name = degree.Name,
                        Description = degree.Description,

                        Branches = _context.GetBranchesById(degree.Branches)
                    };

                    ListOfDegrees.Add(singleDegree);
                }
            }

            List<DisplayUsersViewModel> ListOfUsers = new List<DisplayUsersViewModel>();

            foreach (var user in UsersWithDegree)
            {
                DisplayUsersViewModel singleUser = new DisplayUsersViewModel
                {
                    UserIdentificator = user.Id,

                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    Roles = user.Roles,

                    CompanyRoleManager = _context.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList(),
                    CompanyRoleWorker = _context.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList()
                };

                ListOfUsers.Add(singleUser);
            }

            DegreeDetailsViewModel DegreeDetails = new DegreeDetailsViewModel
            {
                DegreeIdentificator = Degree.DegreeIdentificator,

                DegreeIndexer = Degree.DegreeIndexer,
                Name = Degree.Name,
                Description = Degree.Description,

                Branches = _context.GetBranchesById(Degree.Branches),

                RequiredCertificates = ListOfCertificates,
                RequiredDegrees = ListOfDegrees,
                UsersWithDegree = ListOfUsers
            };

            return View(DegreeDetails);
        }
    }
}