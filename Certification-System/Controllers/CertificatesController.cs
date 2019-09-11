using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public CertificatesController(IGeneratorQR generatorQR, MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: BlankMenu
        [Authorize(Roles = "Instructor")]
        public ActionResult BlankMenu(string message = null)
        {
            ViewBag.message = message;

            return View();
        }

        // GET: AddNewCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCertificate()
        {
            AddCertificateViewModel newCertificate = new AddCertificateViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                SelectedBranches = new List<string>()
            };

            return View(newCertificate);
        }

        // POST: AddNewCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCertificate(AddCertificateViewModel newCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = _mapper.Map<Certificate>(newCertificate);
                certificate.CertificateIdentificator = _keyGenerator.GenerateNewId();

                _context.certificateRepository.AddCertificate(certificate);

                return RedirectToAction("ConfirmationOfActionOnCertificate", new { certificateIdentificator = certificate.CertificateIdentificator, TypeOfAction = "Add" });
            }

            newCertificate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (newCertificate.SelectedBranches == null)
            {
                newCertificate.SelectedBranches = new List<string>();
            }

            return View(newCertificate);
        }

        // GET: ConfirmationOfActionOnCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCertificate(string certificateIdentificator, string TypeOfAction)
        {
            if (certificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);
                DisplayCertificateViewModel modifiedCertificate = _mapper.Map<DisplayCertificateViewModel>(Certificate);

                modifiedCertificate.Branches = _context.branchRepository.GetBranchesById(Certificate.Branches);

                return View(modifiedCertificate);
            }

            return RedirectToAction(nameof(AddNewCertificate));
        }

        // GET: DisplayAllCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCertificates()
        {
            var Certificates = _context.certificateRepository.GetListOfCertificates();

            List<DisplayCertificateViewModel> ListOfCertificates = _mapper.Map<List<DisplayCertificateViewModel>>(Certificates);
            ListOfCertificates.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

            return View(ListOfCertificates);
        }

        // GET: EditCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditCertificate(string certificateIdentificator)
        {
            var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            EditCertificateViewModel certificateToUpdate = _mapper.Map<EditCertificateViewModel>(Certificate);
            certificateToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            return View(certificateToUpdate);
        }

        // POST: EditCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCertificate(EditCertificateViewModel editedCertificate)
        {
            if (ModelState.IsValid)
            {
                Certificate certificate = _mapper.Map<Certificate>(editedCertificate);
                _context.certificateRepository.UpdateCertificate(certificate);

                return RedirectToAction("ConfirmationOfActionOnCertificate", "Certificates", new { certificateIdentificator = editedCertificate.CertificateIdentificator, TypeOfAction = "Update" });
            }

            editedCertificate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (editedCertificate.SelectedBranches == null)
            {
                editedCertificate.SelectedBranches = new List<string>();
            }

            return View(editedCertificate);
        }

        // GET: CertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CertificateDetails(string certificateIdentificator)
        {
            var Certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            var GivenCertificatesInstances = _context.givenCertificateRepository.GetGivenCertificatesByIdOfCertificate(certificateIdentificator);
            var GivenCertificatesIdentificators = GivenCertificatesInstances.Select(z => z.GivenCertificateIdentificator);

            var UsersWithCertificate = _context.userRepository.GetUsersByGivenCertificateId(GivenCertificatesIdentificators.ToList());

            List<DisplayCrucialDataWithCompaniesRoleUserViewModel> ListOfUsers = new List<DisplayCrucialDataWithCompaniesRoleUserViewModel>();

            if (UsersWithCertificate.Count != 0)
            {
                foreach (var user in UsersWithCertificate)
                {
                    DisplayCrucialDataWithCompaniesRoleUserViewModel singleUser = _mapper.Map<DisplayCrucialDataWithCompaniesRoleUserViewModel>(user);
                    singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                    singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                    ListOfUsers.Add(singleUser);
                }
            }

            var CoursesWhichEndedWithSuchCertificate = _context.courseRepository.GetCoursesById(GivenCertificatesInstances.Select(z => z.Course).ToList());

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (CoursesWhichEndedWithSuchCertificate.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(CoursesWhichEndedWithSuchCertificate);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
            }

            CertificateDetailsViewModel CertificateDetails = _mapper.Map<CertificateDetailsViewModel>(Certificate);
            CertificateDetails.Branches = _context.branchRepository.GetBranchesById(Certificate.Branches);
            CertificateDetails.CoursesWhichEndedWithCertificate = ListOfCourses;
            CertificateDetails.UsersWithCertificate = ListOfUsers;

            return View(CertificateDetails);
        }
    }
}