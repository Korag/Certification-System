using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
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

        private readonly IEmailSender _emailSender;
        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public CertificatesController(
            IGeneratorQR generatorQR,
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            ILogService logger,
            IEmailSender emailSender)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: BlankMenu
        [Authorize]
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
                certificate.CertificateIndexer = _keyGenerator.GenerateCertificateEntityIndexer(certificate.Name);

                _context.certificateRepository.AddCertificate(certificate);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCertificateLog(certificate, logInfo);

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
        public ActionResult DisplayAllCertificates(string message = null)
        {
            ViewBag.Message = message;

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

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCertificateLog(certificate, logInfo);

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

        // GET: DeleteCertificateHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCertificateHub(string certificateIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(certificateIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteCertificateEntityLink(certificateIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteCertificate
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCertificate(string certificateIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(certificateIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel certificateToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = certificateIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie certyfikatu"
                };

                return View("DeleteEntity", certificateToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteCertificate(DeleteEntityViewModel certificateToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var certificate = _context.certificateRepository.GetCertificateById(certificateToDelete.EntityIdentificator);

            if (certificate == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, certificateToDelete.Code))
            {
                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.certificateRepository.DeleteCertificate(certificateToDelete.EntityIdentificator);
                _logger.AddCertificateLog(certificate, logInfo);

                var updatedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificatesByCertificateId(certificateToDelete.EntityIdentificator);
                _logger.AddGivenCertificatesLogs(updatedGivenCertificates, logInfoUpdate);

                var updatedGivenDegrees = _context.degreeRepository.DeleteCertificateFromDegrees(certificateToDelete.EntityIdentificator);
                _logger.AddDegreesLogs(updatedGivenDegrees, logInfoUpdate);

                return RedirectToAction("DisplayAllCertificates", "Certificates", new { message = "Usunięto wskazany certyfikat" });
            }

            return View("DeleteEntity", certificateToDelete);
        }
    }
}