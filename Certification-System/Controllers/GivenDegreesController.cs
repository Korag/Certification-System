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
    public class GivenDegreesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public GivenDegreesController(
            MongoOperations context, 
            IGeneratorQR generatorQR, 
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

        // GET: EditGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditGivenDegree(string givenDegreeIdentificator)
        {
            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);
            EditGivenDegreeViewModel givenDegreeToUpdate = _mapper.Map<EditGivenDegreeViewModel>(givenDegree);

            givenDegreeToUpdate.User = _mapper.Map<DisplayCrucialDataUserViewModel>(_context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator));
            givenDegreeToUpdate.Degree = _mapper.Map<DisplayCrucialDataDegreeViewModel>(_context.degreeRepository.GetDegreeById(givenDegree.Degree));

            return View(givenDegreeToUpdate);
        }

        // POST: EditGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenDegree(EditGivenDegreeViewModel editedGivenDegree)
        {
            if (ModelState.IsValid)
            {
                var originGivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(editedGivenDegree.GivenDegreeIdentificator);

                originGivenDegree = _mapper.Map<EditGivenDegreeViewModel, GivenDegree>(editedGivenDegree, originGivenDegree);
                _context.givenDegreeRepository.UpdateGivenDegree(originGivenDegree);

                #region EntityLogs

                var logInfoUpdateGivenDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateGivenDegree"]);
                _logger.AddGivenDegreeLog(originGivenDegree, logInfoUpdateGivenDegree);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateGivenDegree"], "Indekser: " + originGivenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateGivenDegree);

                var logInfoPersonalUpdateUserGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserGivenDegree"], "Indekser: " + originGivenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLog(editedGivenDegree.User.UserIdentificator, logInfoPersonalUpdateUserGivenDegree);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnGivenDegree", "GivenDegrees", new { givenDegreeIdentificator = originGivenDegree.GivenDegreeIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenDegree);
        }

        // GET: ConfirmationOfActionOnGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnGivenDegree(string givenDegreeIdentificator, string TypeOfAction)
        {
            if (givenDegreeIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

                var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                var user = _context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator);

                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);
                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                DisplayGivenDegreeViewModel modifiedGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(givenDegree);
                modifiedGivenDegree.Degree = degreeViewModel;
                modifiedGivenDegree.User = userViewModel;

                return View(modifiedGivenDegree);
            }

            return RedirectToAction(nameof(AddNewGivenDegree));
        }

        // GET: DisplayAllGivenDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenDegrees(string message = null)
        {
            ViewBag.Message = message;

            var givenDegrees = _context.givenDegreeRepository.GetListOfGivenDegrees();
            List<DisplayGivenDegreeViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeViewModel>();

            foreach (var givenDegree in givenDegrees)
            {
                var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                var user = _context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator);

                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);

                DisplayGivenDegreeViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(givenDegree);
                singleGivenDegree.Degree = degreeViewModel;
                singleGivenDegree.User = userViewModel;

                listOfGivenDegrees.Add(singleGivenDegree);
            }

            return View(listOfGivenDegrees);
        }

        // GET: AddNewGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenDegree(string userIdentificator)
        {
            AddGivenDegreeViewModel newGivenDegree = new AddGivenDegreeViewModel
            {
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                newGivenDegree.SelectedUser = userIdentificator;
            }

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

                var degree = _context.degreeRepository.GetDegreeById(newGivenDegree.SelectedDegree);
                givenDegree.GivenDegreeIndexer = _keyGenerator.GenerateGivenDegreeEntityIndexer(degree.DegreeIndexer);

                _context.givenDegreeRepository.AddGivenDegree(givenDegree);
                _context.userRepository.AddUserDegree(newGivenDegree.SelectedUser, givenDegree.GivenDegreeIdentificator);

                var user = _context.userRepository.GetUserById(newGivenDegree.SelectedUser);

                #region EntityLogs

                var logInfoAddGivenDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addGivenDegree"]);
                _logger.AddGivenDegreeLog(givenDegree, logInfoAddGivenDegree);

                var logInfoUpdateUser = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addUserGivenDegree"]);
                _logger.AddUserLog(user, logInfoUpdateUser);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addGivenDegree"], "Indekser: " + givenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddGivenDegree);

                var logInfoPersonalAddUserGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserGivenDegree"], "Indekser: " + givenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalAddUserGivenDegree);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnGivenDegree", new { givenDegreeIdentificator = givenDegree.GivenDegreeIdentificator, TypeOfAction = "Add" });
            }

            newGivenDegree.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();
            newGivenDegree.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();

            return View(newGivenDegree);
        }

        // GET: GivenDegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult GivenDegreeDetails(string givenDegreeIdentificator)
        {
            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

            var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
            var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);

            var user = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

            DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(user);

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);


            List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfRequiredCertificatesWithInstances = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();
            var UsersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

            if (requiredCertificates.Count != 0)
            {
                foreach (var certificate in requiredCertificates)
                {
                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    var requiredGivenCertificate = UsersGivenCertificate.Where(z => z.Certificate == certificate.CertificateIdentificator).FirstOrDefault();

                    DisplayGivenCertificateToUserWithoutCourseViewModel requiredCertificateWithInstance = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(requiredGivenCertificate);
                    requiredCertificateWithInstance.Certificate = certificateViewModel;

                    listOfRequiredCertificatesWithInstances.Add(requiredCertificateWithInstance);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfRequiredDegreesWithInstances = new List<DisplayGivenDegreeToUserViewModel>();
            var usersGivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            if (requiredDegrees.Count != 0)
            {
                foreach (var requiredSingleDegree in requiredDegrees)
                {
                    DisplayCrucialDataDegreeViewModel singleDegreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(requiredSingleDegree);

                    var requiredGivenDegree = usersGivenDegrees.Where(z => z.Degree == degree.DegreeIdentificator).FirstOrDefault();

                    DisplayGivenDegreeToUserViewModel requiredDegreeWithInstance = _mapper.Map<DisplayGivenDegreeToUserViewModel>(singleDegreeViewModel);
                    requiredDegreeWithInstance.Degree = singleDegreeViewModel;

                    listOfRequiredDegreesWithInstances.Add(requiredDegreeWithInstance);
                }
            }

            GivenDegreeDetailsViewModel givenDegreeDetails = new GivenDegreeDetailsViewModel();
            givenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
            givenDegreeDetails.GivenDegree.Degree = degreeViewModel;
            givenDegreeDetails.User = userViewModel;
            givenDegreeDetails.Companies = companiesViewModel;

            givenDegreeDetails.RequiredCertificatesWithGivenInstances = listOfRequiredCertificatesWithInstances;
            givenDegreeDetails.RequiredDegreesWithGivenInstances = listOfRequiredDegreesWithInstances;

            return View(givenDegreeDetails);
        }
        
        // GET: AnonymouslyVerificationOfGivenDegree
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfGivenDegree(string givenDegreeIdentificator)
        {
            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

            var user = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

            DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(user);

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            GivenDegreeDetailsForAnonymousViewModel givenDegreeDetails = new GivenDegreeDetailsForAnonymousViewModel();
            givenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
            givenDegreeDetails.GivenDegree.Degree = degreeViewModel;
            givenDegreeDetails.User = userViewModel;
            givenDegreeDetails.Companies = companiesViewModel;

            return View(givenDegreeDetails);
        }

        // GET: DeleteGivenDegreeHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteGivenDegreeHub(string givenDegreeIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(givenDegreeIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteGivenDegreeEntityLink(givenDegreeIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteGivenDegree(string givenDegreeIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(givenDegreeIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel givenDegreeToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = givenDegreeIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie nadanego stopnia zawodowego"
                };

                return View("DeleteEntity", givenDegreeToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteGivenDegree(DeleteEntityViewModel givenDegreeToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeToDelete.EntityIdentificator);

            if (givenDegree == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, givenDegreeToDelete.Code))
            {
                _context.givenDegreeRepository.DeleteGivenDegree(givenDegreeToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteGivenDegree = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteGivenDegree"]);
                var logInfoUpdateUser = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["deleteUserGivenDegree"]);

                _logger.AddGivenDegreeLog(givenDegree, logInfoDeleteGivenDegree);

                var updatedUser = _context.userRepository.DeleteUserGivenDegree(givenDegreeToDelete.EntityIdentificator);
                _logger.AddUserLog(updatedUser, logInfoUpdateUser);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalDeleteGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenDegree"], "Indekser: " + givenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenDegree);

                var logInfoPersonalDeleteUserGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserGivenDegree"], "Indekser: " + givenDegree.GivenDegreeIndexer);
                _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalDeleteUserGivenDegree);
             
                #endregion

                return RedirectToAction("DisplayAllGivenDegrees", "GivenDegrees", new { message = "Usunięto wskazany nadany stopień zawodowy" });
            }

            return View("DeleteEntity", givenDegreeToDelete);
        }

        // GET: WorkerGivenDegreeDetails
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerGivenDegreeDetails(string givenDegreeIdentificator)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);
            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

            var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
            var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

            List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfRequiredCertificatesWithInstances = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();
            var usersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

            if (requiredCertificates.Count != 0)
            {
                foreach (var certificate in requiredCertificates)
                {
                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    var requiredGivenCertificate = usersGivenCertificate.Where(z => z.Certificate == certificate.CertificateIdentificator).FirstOrDefault();

                    DisplayGivenCertificateToUserWithoutCourseViewModel requiredCertificateWithInstance = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(requiredGivenCertificate);
                    requiredCertificateWithInstance.Certificate = certificateViewModel;

                    listOfRequiredCertificatesWithInstances.Add(requiredCertificateWithInstance);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfRequiredDegreesWithInstances = new List<DisplayGivenDegreeToUserViewModel>();
            var usersGivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            if (requiredDegrees.Count != 0)
            {
                foreach (var requiredSingleDegree in requiredDegrees)
                {
                    DisplayCrucialDataDegreeViewModel singleDegreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(requiredSingleDegree);

                    var requiredGivenDegree = usersGivenDegrees.Where(z => z.Degree == degree.DegreeIdentificator).FirstOrDefault();

                    DisplayGivenDegreeToUserViewModel requiredDegreeWithInstance = _mapper.Map<DisplayGivenDegreeToUserViewModel>(singleDegreeViewModel);
                    requiredDegreeWithInstance.Degree = singleDegreeViewModel;

                    listOfRequiredDegreesWithInstances.Add(requiredDegreeWithInstance);
                }
            }

            WorkerGivenDegreeDetailsViewModel givenDegreeDetails = new WorkerGivenDegreeDetailsViewModel();
            givenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
            givenDegreeDetails.GivenDegree.Degree = degreeViewModel;

            givenDegreeDetails.RequiredCertificatesWithGivenInstances = listOfRequiredCertificatesWithInstances;
            givenDegreeDetails.RequiredDegreesWithGivenInstances = listOfRequiredDegreesWithInstances;

            return View(givenDegreeDetails);
        }

        // GET: CompanyWorkerGivenDegreeDetails
        [Authorize(Roles = "Company")]
        public ActionResult CompanyWorkerGivenDegreeDetails(string givenDegreeIdentificator)
        {
            var user = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);

            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);
            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

            var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
            var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

            List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfRequiredCertificatesWithInstances = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();
            var usersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

            if (requiredCertificates.Count != 0)
            {
                foreach (var certificate in requiredCertificates)
                {
                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    var requiredGivenCertificate = usersGivenCertificate.Where(z => z.Certificate == certificate.CertificateIdentificator).FirstOrDefault();

                    DisplayGivenCertificateToUserWithoutCourseViewModel requiredCertificateWithInstance = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(requiredGivenCertificate);
                    requiredCertificateWithInstance.Certificate = certificateViewModel;

                    listOfRequiredCertificatesWithInstances.Add(requiredCertificateWithInstance);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfRequiredDegreesWithInstances = new List<DisplayGivenDegreeToUserViewModel>();
            var usersGivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            if (requiredDegrees.Count != 0)
            {
                foreach (var requiredSingleDegree in requiredDegrees)
                {
                    DisplayCrucialDataDegreeViewModel singleDegreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(requiredSingleDegree);

                    var requiredGivenDegree = usersGivenDegrees.Where(z => z.Degree == degree.DegreeIdentificator).FirstOrDefault();

                    DisplayGivenDegreeToUserViewModel requiredDegreeWithInstance = _mapper.Map<DisplayGivenDegreeToUserViewModel>(singleDegreeViewModel);
                    requiredDegreeWithInstance.Degree = singleDegreeViewModel;

                    listOfRequiredDegreesWithInstances.Add(requiredDegreeWithInstance);
                }
            }

            CompanyWorkerGivenDegreeDetailsViewModel givenDegreeDetails = new CompanyWorkerGivenDegreeDetailsViewModel();
            givenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
            givenDegreeDetails.GivenDegree.Degree = degreeViewModel;

            givenDegreeDetails.RequiredCertificatesWithGivenInstances = listOfRequiredCertificatesWithInstances;
            givenDegreeDetails.RequiredDegreesWithGivenInstances = listOfRequiredDegreesWithInstances;

            givenDegreeDetails.UserIdentificator = user.Id;

            return View(givenDegreeDetails);
        }
    }
}