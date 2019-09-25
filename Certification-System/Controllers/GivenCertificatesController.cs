using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.Services;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class GivenCertificatesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IEmailSender _emailSender;
        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public GivenCertificatesController(
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

        // GET: DisplayAllGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenCertificates(string message = null)
        {
            ViewBag.Message = message;

            var givenCertificates = _context.givenCertificateRepository.GetListOfGivenCertificates();
            List<DisplayGivenCertificateViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateViewModel>();

            foreach (var givenCertificate in givenCertificates)
            {
                var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);

                DisplayGivenCertificateViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(givenCertificate);
                singleGivenCertificate.Certificate = certificateViewModel;
                singleGivenCertificate.Course = courseViewModel;
                singleGivenCertificate.User = userViewModel;

                listOfGivenCertificates.Add(singleGivenCertificate);
            }

            return View(listOfGivenCertificates);
        }

        // GET: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenCertificate(string userIdentificator)
        {
            AddGivenCertificateViewModel newGivenCertificate = new AddGivenCertificateViewModel
            {
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableCourses = _context.courseRepository.GetAllCoursesAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                var user = _context.userRepository.GetUserById(userIdentificator);

                newGivenCertificate.AvailableCourses = _context.courseRepository.GenerateSelectList(user.Courses).ToList();
            }

            return View(newGivenCertificate);
        }

        // POST: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewGivenCertificate(AddGivenCertificateViewModel newGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                GivenCertificate givenCertificate = _mapper.Map<GivenCertificate>(newGivenCertificate);
                givenCertificate.GivenCertificateIdentificator = _keyGenerator.GenerateNewId();

                givenCertificate.Course = _context.courseRepository.GetCourseById(newGivenCertificate.SelectedCourse).CourseIdentificator;

                var certificate = _context.certificateRepository.GetCertificateById(newGivenCertificate.SelectedCertificate);
                givenCertificate.Certificate = certificate.CertificateIdentificator;
                givenCertificate.GivenCertificateIndexer = _keyGenerator.GenerateGivenCertificateEntityIndexer(certificate.CertificateIndexer);

                _context.givenCertificateRepository.AddGivenCertificate(givenCertificate);
                _context.userRepository.AddUserCertificate(newGivenCertificate.SelectedUser, givenCertificate.GivenCertificateIdentificator);

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddGivenCertificateLog(givenCertificate, logInfoAdd);

                var user = _context.userRepository.GetUserById(newGivenCertificate.SelectedUser);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddUserLog(user, logInfoUpdate);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", new { givenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator, TypeOfAction = "Add" });
            }

            newGivenCertificate.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            newGivenCertificate.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();
            newGivenCertificate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(newGivenCertificate);
        }

        // GET: ConfirmationOfActionOnGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnGivenCertificate(string givenCertificateIdentificator, string TypeOfAction)
        {
            if (givenCertificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

                var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                //var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(User);
                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                DisplayGivenCertificateViewModel modifiedGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(givenCertificate);
                modifiedGivenCertificate.Course = courseViewModel;
                modifiedGivenCertificate.Certificate = certificateViewModel;
                modifiedGivenCertificate.User = userViewModel;

                return View(modifiedGivenCertificate);
            }

            return RedirectToAction(nameof(AddNewGivenCertificate));
        }

        // GET: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditGivenCertificate(string givenCertificateIdentificator)
        {
            var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var ownerOfTheCertificate = _context.userRepository.GetUserByGivenCertificateId(givenCertificateIdentificator);
            var courseWhichEndedWithSuchCertificate = _context.courseRepository.GetCourseById(givenCertificate.Course);
            var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

            EditGivenCertificateViewModel givenCertificateToUpdate = _mapper.Map<EditGivenCertificateViewModel>(givenCertificate);
            givenCertificateToUpdate.User = _mapper.Map<DisplayCrucialDataUserViewModel>(ownerOfTheCertificate);
            givenCertificateToUpdate.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(courseWhichEndedWithSuchCertificate);
            givenCertificateToUpdate.Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

            return View(givenCertificateToUpdate);
        }

        // POST: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenCertificate(EditGivenCertificateViewModel editedGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                var originGivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(editedGivenCertificate.GivenCertificateIdentificator);

                originGivenCertificate = _mapper.Map<EditGivenCertificateViewModel, GivenCertificate>(editedGivenCertificate, originGivenCertificate);
                _context.givenCertificateRepository.UpdateGivenCertificate(originGivenCertificate);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddGivenCertificateLog(originGivenCertificate, logInfo);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", "GivenCertificates", new { givenCertificateIdentificator = originGivenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenCertificate);
        }

        // GET: AnonymouslyVerificationOfGivenCertificate
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfGivenCertificate(string givenCertificateIdentificator)
        {
            var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

            var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(User);

            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

            GivenCertificateDetailsForAnonymousViewModel verifiedGivenCertificate = new GivenCertificateDetailsForAnonymousViewModel();

            verifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
            verifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;
            verifiedGivenCertificate.User = userViewModel;
            verifiedGivenCertificate.Companies = companiesViewModel;

            return View(verifiedGivenCertificate);
        }

        // GET: GivenCertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult GivenCertificateDetails(string givenCertificateIdentificator)
        {
            var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

            var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

            var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            List<string> instructorsIdentificators = new List<string>();
            meetings.ToList().ForEach(z=> z.Instructors.ToList().ForEach(s=> instructorsIdentificators.Add(s)));
            instructorsIdentificators.Distinct();

            var instructors = _context.userRepository.GetInstructorsById(instructorsIdentificators);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(courseViewModel.Branches);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

            foreach (var meeting in meetings)
            {
                DisplayMeetingWithoutCourseViewModel singleMeeting =  _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors.Where(z => meeting.Instructors.Contains(z.Id)).ToList());

                meetingsViewModel.Add(singleMeeting);
            }

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(User);
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(instructors);

            DisplayCertificateViewModel certificateViewModel = _mapper.Map<DisplayCertificateViewModel>(certificate);
            certificateViewModel.Branches = _context.branchRepository.GetBranchesById(certificateViewModel.Branches);

            GivenCertificateDetailsViewModel verifiedGivenCertificate = new GivenCertificateDetailsViewModel();
            verifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseExtendedViewModel>(givenCertificate);
            verifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;

            verifiedGivenCertificate.Course = courseViewModel;
            verifiedGivenCertificate.Meetings = meetingsViewModel;

            verifiedGivenCertificate.User = userViewModel;
            verifiedGivenCertificate.Instructors = instructorsViewModel;
            verifiedGivenCertificate.Companies = companiesViewModel;

            return View(verifiedGivenCertificate);
        }

        // GET: DeleteGivenDegreeHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteGivenCertificateHub(string givenCertificateIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(givenCertificateIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteGivenCertificateEntityLink(givenCertificateIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteGivenCertificate(string givenCertificateIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(givenCertificateIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel givenCertificateToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = givenCertificateIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie nadanego certyfikatu"
                };

                return View("DeleteEntity", givenCertificateToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteGivenCertificate(DeleteEntityViewModel givenCertificateToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateToDelete.EntityIdentificator);

            if (givenCertificate == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, givenCertificateToDelete.Code))
            {
                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.givenCertificateRepository.DeleteGivenCertificate(givenCertificateToDelete.EntityIdentificator);
                _logger.AddGivenCertificateLog(givenCertificate, logInfo);

                var updatedUser = _context.userRepository.DeleteUserGivenCertificate(givenCertificateToDelete.EntityIdentificator);
                _logger.AddUserLog(updatedUser, logInfoUpdate);

                return RedirectToAction("DisplayAllGivenCertificates", "GivenCertificates", new { message = "Usunięto wskazany nadany certyfikat" });
            }

            return View("DeleteEntity", givenCertificateToDelete);
        }
    }
}