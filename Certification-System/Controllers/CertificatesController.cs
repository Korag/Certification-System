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

                #region EntityLogs

                var logInfoAddCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addCertificate"]);
                _logger.AddCertificateLog(certificate, logInfoAddCertificate);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addCertificate"], "Indekser: " + certificate.CertificateIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddCertificate);

                #endregion

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

                var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);
                DisplayCertificateViewModel modifiedCertificate = _mapper.Map<DisplayCertificateViewModel>(certificate);

                modifiedCertificate.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);

                return View(modifiedCertificate);
            }

            return RedirectToAction(nameof(AddNewCertificate));
        }

        // GET: DisplayAllCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCertificates(string message = null)
        {
            ViewBag.Message = message;

            var certificates = _context.certificateRepository.GetListOfCertificates();

            List<DisplayCertificateViewModel> listOfCertificates = _mapper.Map<List<DisplayCertificateViewModel>>(certificates);
            listOfCertificates.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

            return View(listOfCertificates);
        }

        // GET: EditCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditCertificate(string certificateIdentificator)
        {
            var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            EditCertificateViewModel certificateToUpdate = _mapper.Map<EditCertificateViewModel>(certificate);
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
                var originCertificate = _context.certificateRepository.GetCertificateById(editedCertificate.CertificateIdentificator);
                originCertificate = _mapper.Map<EditCertificateViewModel, Certificate>(editedCertificate, originCertificate);

                _context.certificateRepository.UpdateCertificate(originCertificate);

                #region EntityLogs

                var logInfoUpdateCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCertificate"]);
                _logger.AddCertificateLog(originCertificate, logInfoUpdateCertificate);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateCertificate"], "Indekser: " + originCertificate.CertificateIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateCertificate);

                #endregion

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
            var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);

            var givenCertificatesInstances = _context.givenCertificateRepository.GetGivenCertificatesByIdOfCertificate(certificateIdentificator);
            var givenCertificatesIdentificators = givenCertificatesInstances.Select(z => z.GivenCertificateIdentificator);

            var usersWithCertificate = _context.userRepository.GetUsersByGivenCertificatesId(givenCertificatesIdentificators.ToList());

            List<DisplayCrucialDataWithCompaniesRoleUserViewModel> listOfUsers = new List<DisplayCrucialDataWithCompaniesRoleUserViewModel>();

            if (usersWithCertificate.Count != 0)
            {
                foreach (var user in usersWithCertificate)
                {
                    DisplayCrucialDataWithCompaniesRoleUserViewModel singleUser = _mapper.Map<DisplayCrucialDataWithCompaniesRoleUserViewModel>(user);
                    singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                    singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                    listOfUsers.Add(singleUser);
                }
            }

            var coursesWhichEndedWithSuchCertificate = _context.courseRepository.GetCoursesById(givenCertificatesInstances.Select(z => z.Course).ToList());

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (coursesWhichEndedWithSuchCertificate.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(coursesWhichEndedWithSuchCertificate);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
            }

            CertificateDetailsViewModel certificateDetails = _mapper.Map<CertificateDetailsViewModel>(certificate);
            certificateDetails.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);
            certificateDetails.CoursesWhichEndedWithCertificate = listOfCourses;
            certificateDetails.UsersWithCertificate = listOfUsers;

            return View(certificateDetails);
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
                _context.certificateRepository.DeleteCertificate(certificateToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteCertificate"]);
                var logInfoDeleteGivenCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteGivenCertificate"]);
                var logInfoDeleteRequiredCertificateFromGivenDegrees = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteRequiredGivenCertificateFromDegree"]);

                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["deleteUserGivenCertificate"]);

                _logger.AddCertificateLog(certificate, logInfoDeleteCertificate);

                var deletedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificatesByCertificateId(certificateToDelete.EntityIdentificator);
                _logger.AddGivenCertificatesLogs(deletedGivenCertificates, logInfoDeleteGivenCertificate);

                var degreesWhichHasDeletedRequiredCertificate = _context.degreeRepository.DeleteRequiredCertificateFromDegrees(certificateToDelete.EntityIdentificator);
                _logger.AddDegreesLogs(degreesWhichHasDeletedRequiredCertificate, logInfoDeleteRequiredCertificateFromGivenDegrees);

                var updatedUsers = _context.userRepository.GetUsersByGivenCertificatesId(deletedGivenCertificates.Select(z => z.GivenCertificateIdentificator).ToList());
                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalDeleteCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteCertificate"], "Indekser: " + certificate.CertificateIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteCertificate);

                foreach (var deletedGivenCertificate in deletedGivenCertificates)
                {
                    var logInfoPersonalDeleteGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenCertificate);

                    var deletedGivenCertificateOwner = _context.userRepository.GetUserByGivenCertificateId(deletedGivenCertificate.GivenCertificateIdentificator);

                    var logInfoPersonalDeleteUserGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(deletedGivenCertificateOwner.Id, logInfoPersonalDeleteUserGivenCertificate);
                }

                foreach (var deletedGivenCertificate in deletedGivenCertificates)
                {
                    var logInfoPersonalDeleteGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenCertificate);

                    var deletedGivenCertificateOwner = _context.userRepository.GetUserByGivenCertificateId(deletedGivenCertificate.GivenCertificateIdentificator);

                    var logInfoPersonalDeleteUserGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(deletedGivenCertificateOwner.Id, logInfoPersonalDeleteUserGivenCertificate);
                }

                foreach (var degreeWhichHasDeletedRequiredCertificate in degreesWhichHasDeletedRequiredCertificate)
                {
                    var logInfoPersonalUpdateDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteRequiredCertificate"], "Indekser " + degreeWhichHasDeletedRequiredCertificate.DegreeIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateDegree);

                    var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesByIdOfDegree(degreeWhichHasDeletedRequiredCertificate.DegreeIdentificator);

                    foreach (var givenDegree in givenDegrees)
                    {
                        var ownerOfGivenDegree = _context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator);

                        var logInfoPersonalUpdateUserGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserGivenDegree"], "Indekser " + givenDegree.GivenDegreeIndexer);
                        _context.personalLogRepository.AddPersonalUserLog(ownerOfGivenDegree.Id, logInfoPersonalUpdateUserGivenDegree);
                    }
                }

                #endregion

                return RedirectToAction("DisplayAllCertificates", "Certificates", new { message = "Usunięto wskazany certyfikat" });
            }

            return View("DeleteEntity", certificateToDelete);
        }

        // GET: CompanyCertificateDetails
        [Authorize(Roles = "Company")]
        public ActionResult CompanyCertificateDetails(string certificateIdentificator)
        {
            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());

            var certificate = _context.certificateRepository.GetCertificateById(certificateIdentificator);
            var givenCertificatesInstances = _context.givenCertificateRepository.GetGivenCertificatesByIdOfCertificate(certificateIdentificator).Select(z=> z.GivenCertificateIdentificator).ToList();

            var companyWorkersWithCertificate = companyWorkers.Where(z => z.GivenCertificates.Any(x => givenCertificatesInstances.Contains(x))).ToList();

            CompanyCertificateDetailsViewModel certificateDetails = _mapper.Map<CompanyCertificateDetailsViewModel>(certificate);
            certificateDetails.Branches = _context.branchRepository.GetBranchesById(certificate.Branches);
            certificateDetails.UsersWithCertificate = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkersWithCertificate).ToList();

            return View(certificateDetails);
        }
    }
}