using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;

namespace Certification_System.Controllers
{
    public class DegreesController : Controller
    {
        public MongoOperations _context { get; set; }

        public DegreesController(MongoOperations context)
        {
            _context = context;
        }

        // GET: AddNewDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewDegree()
        {
            AddDegreeViewModel newDegree = new AddDegreeViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList(),

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

                _context.degreeRepository.AddDegree(degree);

                return RedirectToAction("AddNewDegreeConfirmation", new { degreeIdentificator = degree.DegreeIdentificator, TypeOfAction = "Add" });
            }

            newDegree.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
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

                var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

                var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);
                var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
                var Branches = _context.branchRepository.GetBranchesById(Degree.Branches);

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
            var Degrees = _context.degreeRepository.GetDegrees();
            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            foreach (var degree in Degrees)
            {
                var RequiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);
                var RequiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);

                DisplayDegreeViewModel singleDegree = new DisplayDegreeViewModel
                {
                    DegreeIdentificator = degree.DegreeIdentificator,

                    DegreeIndexer = degree.DegreeIndexer,
                    Name = degree.Name,
                    Description = degree.Description,

                    RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList(),
                    RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList(),
                    Branches = _context.branchRepository.GetBranchesById(degree.Branches)
                };

                ListOfDegrees.Add(singleDegree);
            }

            return View(ListOfDegrees);
        }

        // GET: EditDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditDegree(string degreeIdentificator)
        {
            var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            AddDegreeViewModel degreeViewModel = new AddDegreeViewModel
            {
                DegreeIdentificator = Degree.DegreeIdentificator,
                DegreeIndexer = Degree.DegreeIndexer,
                Name = Degree.Name,
                Description = Degree.Description,
                SelectedBranches = Degree.Branches,
                SelectedCertificates = Degree.RequiredCertificates,
                SelectedDegrees = Degree.RequiredDegrees,

                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList(),
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
                var OriginDegree = _context.degreeRepository.GetDegreeById(editedDegree.DegreeIdentificator);

                OriginDegree.DegreeIndexer = editedDegree.DegreeIndexer;
                OriginDegree.Name = editedDegree.Name;
                OriginDegree.Description = editedDegree.Description;
                OriginDegree.RequiredCertificates = editedDegree.SelectedCertificates;
                OriginDegree.RequiredDegrees = editedDegree.SelectedDegrees;
                OriginDegree.Branches = editedDegree.SelectedBranches;

                _context.degreeRepository.UpdateDegree(OriginDegree);

                return RedirectToAction("AddNewDegreeConfirmation", "Degrees", new { degreeIdentificator = editedDegree.DegreeIdentificator, TypeOfAction = "Update" });
            }

            editedDegree.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            editedDegree.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            editedDegree.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();

            return View(editedDegree);
        }

        // GET: DegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult DegreeDetails(string degreeIdentificator)
        {
            var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
            var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);

            var GivenDegreesInstances = _context.givenDegreeRepository.GetGivenDegreesByIdOfDegree(degreeIdentificator);
            var GivenDegreesIdentificators = GivenDegreesInstances.Select(z => z.GivenDegreeIdentificator);
            var UsersWithDegree = _context.userRepository.GetUsersByGivenDegreeId(GivenDegreesIdentificators.ToList());

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
                        Branches = _context.branchRepository.GetBranchesById(certificate.Branches),
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

                        Branches = _context.branchRepository.GetBranchesById(degree.Branches)
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

                    CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList(),
                    CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList()
                };

                ListOfUsers.Add(singleUser);
            }

            DegreeDetailsViewModel DegreeDetails = new DegreeDetailsViewModel
            {
                DegreeIdentificator = Degree.DegreeIdentificator,

                DegreeIndexer = Degree.DegreeIndexer,
                Name = Degree.Name,
                Description = Degree.Description,

                Branches = _context.branchRepository.GetBranchesById(Degree.Branches),

                RequiredCertificates = ListOfCertificates,
                RequiredDegrees = ListOfDegrees,
                UsersWithDegree = ListOfUsers
            };

            return View(DegreeDetails);
        }
    }
}