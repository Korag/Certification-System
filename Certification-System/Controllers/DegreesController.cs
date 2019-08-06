using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;

namespace Certification_System.Controllers
{
    public class DegreesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public DegreesController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewDegree(AddDegreeViewModel newDegree)
        {
            if (ModelState.IsValid)
            {
                Degree degree = _mapper.Map<Degree>(newDegree);
                degree.DegreeIdentificator = _keyGenerator.GenerateNewId();

                _context.degreeRepository.AddDegree(degree);

                return RedirectToAction("ConfirmationOfActionOnDegree", new { degreeIdentificator = degree.DegreeIdentificator, TypeOfAction = "Add" });
            }

            newDegree.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (newDegree.SelectedBranches == null)
            {
                newDegree.SelectedBranches = new List<string>();
            }

            return View(newDegree);
        }

        // GET: ConfirmationOfActionOnDegree
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnDegree(string degreeIdentificator, string TypeOfAction)
        {
            if (degreeIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

                var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);
                var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
                var Branches = _context.branchRepository.GetBranchesById(Degree.Branches);

                DisplayDegreeViewModel modifiedDegree = _mapper.Map<DisplayDegreeViewModel>(Degree);

                modifiedDegree.RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                modifiedDegree.RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIdentificator + " " + z.Name).ToList();
                modifiedDegree.Branches = Branches;

                return View(modifiedDegree);
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

                DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);

                singleDegree.RequiredCertificates = RequiredCertificates.Select(z => z.CertificateIndexer + " " + z.Name).ToList();
                singleDegree.RequiredDegrees = RequiredDegrees.Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);


                ListOfDegrees.Add(singleDegree);
            }

            return View(ListOfDegrees);
        }

        // GET: EditDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditDegree(string degreeIdentificator)
        {
            var Degree = _context.degreeRepository.GetDegreeById(degreeIdentificator);

            EditDegreeViewModel degreeViewModel = _mapper.Map<EditDegreeViewModel>(Degree);

            degreeViewModel.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            degreeViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            degreeViewModel.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();

            return View(degreeViewModel);
        }

        // POST: EditDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditDegree(EditDegreeViewModel editedDegree)
        {
            if (ModelState.IsValid)
            {
                var OriginDegree = _context.degreeRepository.GetDegreeById(editedDegree.DegreeIdentificator);
                OriginDegree =_mapper.Map<EditDegreeViewModel, Degree>(editedDegree, OriginDegree);

                _context.degreeRepository.UpdateDegree(OriginDegree);

                return RedirectToAction("ConfirmationOfActionOnDegree", "Degrees", new { degreeIdentificator = editedDegree.DegreeIdentificator, TypeOfAction = "Update" });
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
            var UsersWithDegree = _context.userRepository.GetUsersByDegreeId(GivenDegreesIdentificators.ToList());

            List<DisplayCertificateViewModel> ListOfCertificates = new List<DisplayCertificateViewModel>();

            if (RequiredCertificates.Count != 0)
            {
                foreach (var certificate in RequiredCertificates)
                {
                    DisplayCertificateViewModel singleCertificate = _mapper.Map<DisplayCertificateViewModel>(certificate);
                    singleCertificate.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);

                    ListOfCertificates.Add(singleCertificate);
                }
            }

            List<DisplayDegreeViewModel> ListOfDegrees = new List<DisplayDegreeViewModel>();

            if (RequiredDegrees.Count != 0)
            {
                foreach (var degree in RequiredDegrees)
                {
                    DisplayDegreeViewModel singleDegree = _mapper.Map<DisplayDegreeViewModel>(degree);
                    singleDegree.Branches = _context.branchRepository.GetBranchesById(degree.Branches);
                    singleDegree.RequiredCertificates = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees).Select(z => z.DegreeIndexer + " " + z.Name).ToList();
                    singleDegree.RequiredDegrees = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates).Select(z => z.CertificateIndexer + " " + z.Name).ToList();

                    ListOfDegrees.Add(singleDegree);
                }
            }

            List<DisplayUserViewModel> ListOfUsers = new List<DisplayUserViewModel>();

            foreach (var user in UsersWithDegree)
            {
                DisplayUserViewModel singleUser = _mapper.Map<DisplayUserViewModel>(user);
                singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                ListOfUsers.Add(singleUser);
            }

            DegreeDetailsViewModel DegreeDetails = _mapper.Map<DegreeDetailsViewModel>(Degree);
            DegreeDetails.Branches = _context.branchRepository.GetBranchesById(Degree.Branches);
            DegreeDetails.RequiredCertificates = ListOfCertificates;
            DegreeDetails.RequiredDegrees = ListOfDegrees;
            DegreeDetails.UsersWithDegree = ListOfUsers;

            return View(DegreeDetails);
        }

        // GET: DisplayAllGivenDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenDegrees()
        {
            var GivenDegrees = _context.givenDegreeRepository.GetGivenDegrees();
            List<DisplayGivenDegreeViewModel> ListOfGivenDegrees= new List<DisplayGivenDegreeViewModel>();

            foreach (var givenDegree in GivenDegrees)
            {
                var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                var User = _context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator);

                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);
                DisplayCrucialDataUsersViewModel userViewModel = _mapper.Map<DisplayCrucialDataUsersViewModel>(User);

                DisplayGivenDegreeViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(givenDegree);
                singleGivenDegree.Degree = degreeViewModel;
                singleGivenDegree.User = userViewModel;

                ListOfGivenDegrees.Add(singleGivenDegree);
            }

            return View(ListOfGivenDegrees);
        }

        // GET: AddNewGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenDegree()
        {
            AddGivenDegreeViewModel newGivenDegree = new AddGivenDegreeViewModel
            {
                AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList()
            };

            return View(newGivenDegree);
        }

        // POST: AddNewGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewGivenDegree(AddGivenDegreeViewModel newGivenDegree)
        {
            if (ModelState.IsValid)
            {
                GivenDegree givenDegree = _mapper.Map<GivenDegree>(newGivenDegree);
                givenDegree.GivenDegreeIdentificator = _keyGenerator.GenerateNewId();
                givenDegree.Degree = _context.degreeRepository.GetDegreeById(newGivenDegree.SelectedDegree).DegreeIdentificator;

                _context.givenDegreeRepository.AddGivenDegree(givenDegree);
                _context.userRepository.AddUserDegree(newGivenDegree.SelectedUser, givenDegree.GivenDegreeIdentificator);

                return RedirectToAction("ConfirmationOfActionOnGivenDegree", new { givenDegreeIdentificator = givenDegree.GivenDegreeIdentificator, TypeOfAction = "Add" });
            }

            newGivenDegree.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();
            newGivenDegree.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();

            return View(newGivenDegree);
        }
    }
}