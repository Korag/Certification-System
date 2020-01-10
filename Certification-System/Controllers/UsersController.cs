using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo.Model;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using Certification_System.Extensions;

namespace Certification_System.Controllers
{
    public class UsersController : Controller
    {
        private readonly MongoOperations _context;

        private readonly UserManager<CertificationPlatformUser> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IEmailSender _emailSender;
        private readonly ILogService _logger;

        public UsersController(
            UserManager<CertificationPlatformUser> userManager,
            RoleManager<MongoRole> roleManager,
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            IEmailSender emailSender,
            ILogService logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: DisplayAllUsers
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllUsers(string message = null)
        {
            ViewBag.Message = message;

            var users = _context.userRepository.GetListOfUsers();
            List<DisplayUserViewModel> usersToDisplay = new List<DisplayUserViewModel>();

            ViewBag.AvailableRoleFilters = _context.userRepository.GetAvailableRoleFiltersAsSelectList();

            foreach (var user in users)
            {
                DisplayUserViewModel singleUser = _mapper.Map<DisplayUserViewModel>(user);

                singleUser.Roles = _context.userRepository.TranslateRoles(singleUser.Roles);

                singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                usersToDisplay.Add(singleUser);
            }

            return View(usersToDisplay);
        }

        // GET: ConfirmationOfActionOnUser
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnUser(string userIdentificator, string TypeOfAction)
        {
            if (userIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var user = _context.userRepository.GetUserById(userIdentificator);

                DisplayAllUserInformationViewModel modifiedUser = _mapper.Map<DisplayAllUserInformationViewModel>(user);

                modifiedUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(z => z.CompanyName).ToList();
                modifiedUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(z => z.CompanyName).ToList();

                modifiedUser.Roles = _context.userRepository.TranslateRoles(modifiedUser.Roles);

                return View(modifiedUser);
            }

            return RedirectToAction(nameof(AddNewUser));
        }

        // GET: AddNewUser
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewUser()
        {
            AddUserViewModel newUser = new AddUserViewModel
            {
                AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList(),
                AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList()
            };

            newUser.SelectedRole = newUser.AvailableRoles.Where(z => z.Value == "Worker").Select(z => z.Value).FirstOrDefault();

            return View(newUser);
        }

        // POST: AddNewUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(AddUserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                CertificationPlatformUser user = _mapper.Map<CertificationPlatformUser>(newUser);
                user.Id = _keyGenerator.GenerateNewId();
                user.SecurityStamp = _keyGenerator.GenerateNewGuid();

                if (!await _roleManager.RoleExistsAsync(newUser.SelectedRole))
                {
                    await _roleManager.CreateAsync(new CertificationPlatformUserRole(newUser.SelectedRole));
                }

                if (newUser.SelectedRole != "Instructor&Examiner")
                {
                    var addToRole = await _userManager.AddToRoleAsync(user, newUser.SelectedRole);
                }
                else
                {
                    var addToFirstRole = await _userManager.AddToRoleAsync(user, "Instructor");
                    var addToSecondRole = await _userManager.AddToRoleAsync(user, "Examiner");
                }

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var callbackUrl = Url.SetUserPasswordLink(user.Id, Request.Scheme);

                    var emailToSend = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "setPassword", callbackUrl);
                    await _emailSender.SendEmailAsync(emailToSend);

                    #region EntityLogs

                    var logInfoAddUser = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addUser"]);
                    _logger.AddUserLog(user, logInfoAddUser);

                    #endregion

                    #region PersonalUserLogs

                    var createdUser = _context.userRepository.GetUserById(user.Id);
                    _context.personalLogRepository.CreatePersonalUserLog(createdUser);

                    var logInfoPersonalAddUser = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUser"]);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUser);

                    var logInfoPersonalUserCreation = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["userCreation"]);
                    _context.personalLogRepository.AddPersonalUserLog(createdUser.Id, logInfoPersonalUserCreation);

                    #endregion

                    return RedirectToAction("ConfirmationOfActionOnUser", "Users", new { userIdentificator = user.Id, TypeOfAction = "Add" });
                }

                ModelState.AddModelError(string.Empty, "Użytkownik o podanym adresie email już widnieje w systemie.");
                newUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
                newUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

                return View(newUser);
            }

            newUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
            newUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

            return View(newUser);
        }

        // GET: EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            EditUserViewModel userToUpdate = _mapper.Map<EditUserViewModel>(user);
            userToUpdate.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();
            userToUpdate.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();

            if (userToUpdate.SelectedRole.Contains("INSTRUCTOR") && userToUpdate.SelectedRole.Contains("EXAMINER"))
            {
                userToUpdate.SelectedRole.Clear();
                userToUpdate.SelectedRole.Add("Instructor&Examiner");
            }

            return View(userToUpdate);
        }

        // POST: EditUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel editedUser)
        {
            if (ModelState.IsValid)
            {
                var originUser = _userManager.FindByIdAsync(editedUser.UserIdentificator).Result;
                _userManager.RemoveFromRolesAsync(originUser, _userManager.GetRolesAsync(originUser).Result.ToArray()).Wait();

                if (editedUser.SelectedRole.FirstOrDefault() != "Instructor&Examiner")
                {
                    _userManager.AddToRolesAsync(originUser, editedUser.SelectedRole).Wait();
                }
                else
                {
                    _userManager.AddToRoleAsync(originUser, "Instructor").Wait();
                    _userManager.AddToRoleAsync(originUser, "Examiner").Wait();
                }

                if (originUser.Email != editedUser.Email)
                {
                    originUser.EmailConfirmed = false;
                }

                originUser = _mapper.Map<EditUserViewModel, CertificationPlatformUser>(editedUser, originUser);

                _context.userRepository.UpdateUser(originUser);

                var updatedUser = _context.userRepository.GetUserById(originUser.Id);

                #region EntityLogs

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateUser"]);
                _logger.AddUserLog(updatedUser, logInfo);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateUser = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUser"]);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateUser);

                var logInfoPersonalUserDataModification = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["userDataModification"]);
                _context.personalLogRepository.AddPersonalUserLog(updatedUser.Id, logInfoPersonalUserDataModification);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnUser", "Users", new { userIdentificator = originUser.Id, TypeOfAction = "Update" });
            }

            editedUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
            editedUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

            return View(editedUser);
        }

        // GET: UserDetails
        [Authorize(Roles = "Admin")]
        public ActionResult UserDetails(string userIdentificator, string message)
        {
            ViewBag.Message = message;

            var user = _context.userRepository.GetUserById(userIdentificator);
            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
            var courses = _context.courseRepository.GetCoursesById(user.Courses);
            var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            var companiesRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker);
            var companiesRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager);

            List<Company> companies = companiesRoleWorker.ToList();

            foreach (var company in companiesRoleManager)
            {
                if (companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    companies.Add(company);
                }
            }

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                foreach (var course in courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    listOfCourses.Add(singleCourse);
                }
            }

            List<DisplayGivenCertificateToUserViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

            if (givenCertificates.Count != 0)
            {
                foreach (var givenCertificate in givenCertificates)
                {
                    var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;
                    singleGivenCertificate.Course = courseViewModel;

                    listOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (givenDegrees.Count != 0)
            {
                foreach (var givenDegree in givenDegrees)
                {
                    var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    listOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> listOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            UserDetailsViewModel userDetails = _mapper.Map<UserDetailsViewModel>(user);
            userDetails.Roles = _context.userRepository.TranslateRoles(userDetails.Roles);

            userDetails.GivenCertificates = listOfGivenCertificates;
            userDetails.GivenDegrees = listOfGivenDegrees;
            userDetails.Courses = listOfCourses;
            userDetails.Companies = listOfCompanies;

            return View(userDetails);
        }

        // GET: CompanyWorkerDetails
        [Authorize(Roles = "Company")]
        public ActionResult CompanyWorkerDetails(string userIdentificator, string message = null)
        {
            ViewBag.message = message;
            
            var user = _context.userRepository.GetUserById(userIdentificator);
            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
            var courses = _context.courseRepository.GetCoursesById(user.Courses);
            var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                foreach (var course in courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    listOfCourses.Add(singleCourse);
                }
            }

            List<DisplayGivenCertificateToUserViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

            if (givenCertificates.Count != 0)
            {
                foreach (var givenCertificate in givenCertificates)
                {
                    var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;
                    singleGivenCertificate.Course = courseViewModel;

                    listOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (givenDegrees.Count != 0)
            {
                foreach (var givenDegree in givenDegrees)
                {
                    var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    listOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            CompanyWorkerDetailsViewModel userDetails = _mapper.Map<CompanyWorkerDetailsViewModel>(user);

            userDetails.GivenCertificates = listOfGivenCertificates;
            userDetails.GivenDegrees = listOfGivenDegrees;
            userDetails.Courses = listOfCourses;

            return View(userDetails);
        }

        // GET: AnonymouslyVerificationOfUser
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfUser(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);
            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
            var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            var companiesRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker);
            var companiesRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager);

            List<Company> companies = companiesRoleWorker.ToList();

            foreach (var company in companiesRoleManager)
            {
                if (companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    companies.Add(company);
                }
            }

            List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();

            if (givenCertificates.Count != 0)
            {
                foreach (var givenCertificate in givenCertificates)
                {
                    var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    DisplayGivenCertificateToUserWithoutCourseViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;

                    listOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (givenDegrees.Count != 0)
            {
                foreach (var givenDegree in givenDegrees)
                {
                    var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    listOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

            UserDetailsForAnonymousViewModel verifiedUser = _mapper.Map<UserDetailsForAnonymousViewModel>(user);
            verifiedUser.GivenCertificates = listOfGivenCertificates;
            verifiedUser.GivenDegrees = listOfGivenDegrees;
            verifiedUser.Companies = ListOfCompanies;

            return View(verifiedUser);
        }

        // GET: InstructorDetails
        [Authorize(Roles = "Admin")]
        public ActionResult InstructorDetails(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var meetings = _context.meetingRepository.GetMeetingsByInstructorId(userIdentificator);
            var courses = _context.courseRepository.GetCoursesByMeetingsId(meetings.Select(z => z.MeetingIdentificator).ToList());

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                foreach (var course in courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    listOfCourses.Add(singleCourse);
                }
            }

            List<DisplayMeetingWithoutInstructorViewModel> listOfMeetings = new List<DisplayMeetingWithoutInstructorViewModel>();

            if (meetings.Count != 0)
            {
                foreach (var meeting in meetings)
                {
                    DisplayMeetingWithoutInstructorViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutInstructorViewModel>(meeting);
                    singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(courses.Where(z => z.Meetings.Contains(meeting.MeetingIdentificator)).FirstOrDefault());

                    listOfMeetings.Add(singleMeeting);
                }
            }

            InstructorDetailsViewModel instructorDetails = _mapper.Map<InstructorDetailsViewModel>(user);
            instructorDetails.Roles = _context.userRepository.TranslateRoles(instructorDetails.Roles);

            instructorDetails.Courses = listOfCourses;
            instructorDetails.Meetings = listOfMeetings;

            return View(instructorDetails);
        }

        // GET: AccountDetails
        [Authorize]
        public ActionResult AccountDetails(string userIdentificator)
        {
            CertificationPlatformUser user;

            if (userIdentificator == null)
            {
                user = _userManager.FindByEmailAsync(this.User.Identity.Name).Result;
            }
            else
            {
                user = _context.userRepository.GetUserById(userIdentificator);
            }

            AccountDetailsViewModel accountDetails = _mapper.Map<AccountDetailsViewModel>(user);
            accountDetails.Roles = _context.userRepository.TranslateRoles(accountDetails.Roles);

            return View(accountDetails);
        }

        // GET: CompanyWithAccountDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CompanyWithAccountDetails(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            if (user == null)
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var company = _context.companyRepository.GetCompanyById(user.CompanyRoleManager.FirstOrDefault());
            var usersConnectedToCompany = _context.userRepository.GetUsersConnectedToCompany(company.CompanyIdentificator);

            List<DisplayCrucialDataWithCompaniesRoleUserViewModel> listOfUsers = new List<DisplayCrucialDataWithCompaniesRoleUserViewModel>();

            if (usersConnectedToCompany.Count != 0)
            {
                listOfUsers = _mapper.Map<List<DisplayCrucialDataWithCompaniesRoleUserViewModel>>(usersConnectedToCompany);

                listOfUsers.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).ToList().Select(s => s.CompanyName).ToList());
                listOfUsers.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).ToList().Select(s => s.CompanyName).ToList());
            }

            CompanyWithAccountDetailsViewModel companyWithAccountDetails = _mapper.Map<CompanyWithAccountDetailsViewModel>(company);
            companyWithAccountDetails.UsersConnectedToCompany = listOfUsers;
            companyWithAccountDetails.UserAccount = _mapper.Map<AccountDetailsViewModel>(user);

            companyWithAccountDetails.UserAccount.Roles = _context.userRepository.TranslateRoles(companyWithAccountDetails.UserAccount.Roles);

            return View(companyWithAccountDetails);
        }

        // GET: EditAccount
        [Authorize]
        public ActionResult EditAccount(string userIdentificator, string message)
        {
            ViewBag.message = message;

            CertificationPlatformUser user = _context.userRepository.GetUserById(userIdentificator);

            EditAccountViewModel accountDetails = _mapper.Map<EditAccountViewModel>(user);

            return View(accountDetails);
        }

        // POST: EditAccount
        [Authorize]
        [HttpPost]
        public ActionResult EditAccount(EditAccountViewModel editedAccount)
        {
            if (ModelState.IsValid)
            {
                var originUser = _userManager.FindByIdAsync(editedAccount.UserIdentificator).Result;

                if (originUser.Email != editedAccount.Email)
                {
                    originUser.EmailConfirmed = false;
                }

                originUser = _mapper.Map<EditAccountViewModel, CertificationPlatformUser>(editedAccount, originUser);

                _context.userRepository.UpdateUser(originUser);

                #region EntityLogs

                var updatedUser = _context.userRepository.GetUserById(originUser.Id);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateUser"]);
                _logger.AddUserLog(updatedUser, logInfo);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateUser = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUser"]);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateUser);

                var logInfoPersonalUserDataModification = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["userDataModification"]);
                _context.personalLogRepository.AddPersonalUserLog(updatedUser.Id, logInfoPersonalUserDataModification);

                #endregion

                return RedirectToAction("EditAccount", "Users", new { userIdentificator = originUser.Id, message = "Pomyślnie zaktualizowano dane" });
            }

            return View(editedAccount);
        }

        // GET: ExaminerDetails
        [Authorize(Roles = "Admin")]
        public ActionResult ExaminerDetails(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var exams = _context.examRepository.GetExamsByExaminerId(userIdentificator);
            var examsTerms = _context.examTermRepository.GetExamTermsByExaminerId(userIdentificator);

            var courses = _context.courseRepository.GetExaminerCourses(userIdentificator, exams);

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                foreach (var course in courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    listOfCourses.Add(singleCourse);
                }
            }

            List<DisplayExamWithoutExaminerViewModel> listOfExams = new List<DisplayExamWithoutExaminerViewModel>();

            if (exams.Count != 0)
            {
                foreach (var exam in exams)
                {
                    DisplayExamWithoutExaminerViewModel singleExam = _mapper.Map<DisplayExamWithoutExaminerViewModel>(exam);
                    singleExam.UsersQuantitiy = exam.EnrolledUsers.Count();
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(courses.ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)).FirstOrDefault());

                    listOfExams.Add(singleExam);
                }
            }

            List<DisplayExamTermWithoutExaminerViewModel> listOfExamsTerms = new List<DisplayExamTermWithoutExaminerViewModel>();

            if (examsTerms.Count != 0)
            {
                foreach (var examTerm in examsTerms)
                {
                    DisplayExamTermWithoutExaminerViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExaminerViewModel>(examTerm);
                    singleExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exams.Where(z => z.ExamDividedToTerms == true && z.ExamTerms.Contains(examTerm.ExamTermIdentificator)).FirstOrDefault());

                    listOfExamsTerms.Add(singleExamTerm);
                }
            }

            ExaminerDetailsViewModel examinerDetails = _mapper.Map<ExaminerDetailsViewModel>(user);
            examinerDetails.Roles = _context.userRepository.TranslateRoles(examinerDetails.Roles);

            examinerDetails.Courses = listOfCourses;
            examinerDetails.Exams = listOfExams;
            examinerDetails.ExamsTerms = listOfExamsTerms;

            return View(examinerDetails);
        }

        // GET: InstructorExaminerDetails
        [Authorize(Roles = "Admin")]
        public ActionResult InstructorExaminerDetails(string userIdentificator)
        {
            ViewBag.AvailableRoleFilters = _context.userRepository.GetAvailableCourseRoleFiltersAsSelectList();

            var user = _context.userRepository.GetUserById(userIdentificator);

            var exams = _context.examRepository.GetExamsByExaminerId(userIdentificator);
            var examsTerms = _context.examTermRepository.GetExamTermsByExaminerId(userIdentificator);

            var meetings = _context.meetingRepository.GetMeetingsByInstructorId(userIdentificator);
            var coursesAsInstructor = _context.courseRepository.GetCoursesByMeetingsId(meetings.Select(z => z.MeetingIdentificator).ToList());
            var coursesAsExaminer = _context.courseRepository.GetExaminerCourses(userIdentificator, exams);

            var bothRolesCourses = coursesAsExaminer.Intersect(coursesAsInstructor).ToList();

            foreach (var course in bothRolesCourses)
            {
                coursesAsExaminer.Remove(course);
                coursesAsInstructor.Remove(course);
            }

            List<DisplayCourseWithUserRoleViewModel> listOfCoursesAsExaminer = new List<DisplayCourseWithUserRoleViewModel>();

            if (coursesAsExaminer.Count != 0)
            {
                listOfCoursesAsExaminer = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(coursesAsExaminer);
                listOfCoursesAsExaminer.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                listOfCoursesAsExaminer.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Examiner"]));
            }

            List<DisplayCourseWithUserRoleViewModel> listOfCoursesAsInstructor = new List<DisplayCourseWithUserRoleViewModel>();

            if (coursesAsInstructor.Count != 0)
            {
                listOfCoursesAsInstructor = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(coursesAsInstructor);
                listOfCoursesAsInstructor.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                listOfCoursesAsInstructor.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Instructor"]));
            }

            List<DisplayCourseWithUserRoleViewModel> listOfCoursesAsBothRoles = new List<DisplayCourseWithUserRoleViewModel>();

            if (bothRolesCourses.Count != 0)
            {
                listOfCoursesAsBothRoles = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(coursesAsInstructor);
                listOfCoursesAsBothRoles.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

                listOfCoursesAsBothRoles.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Instructor"]));
                listOfCoursesAsBothRoles.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Examiner"]));
            }

            List<DisplayCourseWithUserRoleViewModel> listOfCourses = new List<DisplayCourseWithUserRoleViewModel>();

            listOfCourses.AddRange(listOfCoursesAsExaminer);
            listOfCourses.AddRange(listOfCoursesAsInstructor);
            listOfCourses.AddRange(listOfCoursesAsBothRoles);

            List<DisplayMeetingWithoutInstructorViewModel> listOfMeetings = new List<DisplayMeetingWithoutInstructorViewModel>();

            if (meetings.Count != 0)
            {
                foreach (var meeting in meetings)
                {
                    DisplayMeetingWithoutInstructorViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutInstructorViewModel>(meeting);
                    singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(coursesAsInstructor.Where(z => z.Meetings.Contains(meeting.MeetingIdentificator)).FirstOrDefault());

                    listOfMeetings.Add(singleMeeting);
                }
            }

            List<DisplayExamWithoutExaminerViewModel> listOfExams = new List<DisplayExamWithoutExaminerViewModel>();

            if (exams.Count != 0)
            {
                foreach (var exam in exams)
                {
                    DisplayExamWithoutExaminerViewModel singleExam = _mapper.Map<DisplayExamWithoutExaminerViewModel>(exam);
                    singleExam.UsersQuantitiy = exam.EnrolledUsers.Count();
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(coursesAsExaminer.ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)).FirstOrDefault());

                    listOfExams.Add(singleExam);
                }
            }

            List<DisplayExamTermWithoutExaminerViewModel> listOfExamsTerms = new List<DisplayExamTermWithoutExaminerViewModel>();

            if (examsTerms.Count != 0)
            {
                foreach (var examTerm in examsTerms)
                {
                    DisplayExamTermWithoutExaminerViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExaminerViewModel>(examTerm);
                    var examRelatedWithExamTerm = exams.Where(z => z.ExamTerms.Contains(examTerm.ExamTermIdentificator)).FirstOrDefault();
                    singleExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(examRelatedWithExamTerm);

                    listOfExamsTerms.Add(singleExamTerm);
                }
            }

            InstructorExaminerDetailsViewModel instructorExaminerDetails = _mapper.Map<InstructorExaminerDetailsViewModel>(user);
            instructorExaminerDetails.Roles = _context.userRepository.TranslateRoles(instructorExaminerDetails.Roles);

            instructorExaminerDetails.Courses = listOfCourses;
            instructorExaminerDetails.Meetings = listOfMeetings;

            instructorExaminerDetails.Exams = listOfExams;
            instructorExaminerDetails.ExamsTerms = listOfExamsTerms;

            return View(instructorExaminerDetails);
        }

        // GET: DeleteUserHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteUserHub(string userIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteUserEntityLink(userIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteUser
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteUser(string userIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(userIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel userToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = userIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie użytkownika systemu"
                };

                return View("DeleteEntity", userToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteUser(DeleteEntityViewModel userToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var userToDeleteModel = _context.userRepository.GetUserById(userToDelete.EntityIdentificator);

            if (userToDeleteModel == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, userToDelete.Code))
            {
                _context.userRepository.DeleteUser(userToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteUser = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteUser"]);
                var logInfoDeleteGivenDegrees = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteGivenDegree"]);
                var logInfoDeleteGivenCertificates = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteGivenCertificate"]);
                var logInfoDeleteExamsResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamResult"]);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromCourse"]);
                var logInfoUpdateMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromMeeting"]);
                var logInfoUpdateExams = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromExam"]);
                var logInfoUpdateExamsTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromExamTerm"]);

                _logger.AddUserLog(userToDeleteModel, logInfoDeleteUser);

                var deletedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificates(userToDeleteModel.GivenCertificates);
                _logger.AddGivenCertificatesLogs(deletedGivenCertificates, logInfoDeleteGivenCertificates);

                var deletedGivenDegrees = _context.givenDegreeRepository.DeleteGivenDegrees(userToDeleteModel.GivenDegrees);
                _logger.AddGivenDegreesLogs(deletedGivenDegrees, logInfoDeleteGivenDegrees);

                var updatedCourses = _context.courseRepository.DeleteUserFromCourses(userToDeleteModel.Courses);
                _logger.AddCoursesLogs(updatedCourses, logInfoUpdateCourse);

                var updatedMeetings = _context.meetingRepository.DeleteUserFromMeetings(userToDeleteModel.Id, updatedCourses.SelectMany(z => z.Meetings).ToList());
                _logger.AddMeetingsLogs(updatedMeetings, logInfoUpdateMeetings);

                var updatedExams = _context.examRepository.DeleteUserFromExams(userToDeleteModel.Id, updatedCourses.SelectMany(z => z.Exams).ToList());
                _logger.AddExamsLogs(updatedExams, logInfoUpdateExams);

                var updatedExamsTerms = _context.examTermRepository.DeleteUserFromExamsTerms(userToDeleteModel.Id, updatedExams.SelectMany(z => z.ExamTerms).ToList());
                _logger.AddExamsTermsLogs(updatedExamsTerms, logInfoUpdateExamsTerms);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResultsByUserId(userToDelete.EntityIdentificator);
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDeleteExamsResults);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalDeleteUser = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUser"], "Email: " + user.Email);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteUser);

                var logInfoPersonalDeleteUserInformation = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserInformation"], "Email: " + user.Email);
                _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalDeleteUserInformation);

                foreach (var deletedGivenCertificate in deletedGivenCertificates)
                {
                    var logInfoPersonalDeleteGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenCertificate"], "Indekser: " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenCertificate);
                }

                foreach (var deletedGivenDegree in deletedGivenDegrees)
                {
                    var logInfoPersonalDeleteGivenDegree = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenDegree"], "Indekser: " + deletedGivenDegree.GivenDegreeIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenDegree);
                }

                foreach (var updatedCourse in updatedCourses)
                {
                    var logInfoPersonalRemoveUserFromCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeUserFromCourse"], "Indekser: " + updatedCourse.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalRemoveUserFromCourse);
                }

                foreach (var updatedMeeting in updatedMeetings)
                {
                    var logInfoPersonalRemoveUserFromMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeUserFromMeeting"], "Indekser: " + updatedMeeting.MeetingIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalRemoveUserFromMeeting);
                }

                foreach (var updatedExam in updatedExams)
                {
                    var logInfoPersonalRemoveUserFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeUserFromExam"], "Indekser: " + updatedExam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalRemoveUserFromExam);
                }

                foreach (var updatedExamTerm in updatedExamsTerms)
                {
                    var logInfoPersonalRemoveUserFromExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeUserFromExamTerm"], "Indekser: " + updatedExamTerm.ExamTermIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalRemoveUserFromExamTerm);
                }

                foreach (var deleteExamResult in deletedExamsResults)
                {
                    var logInfoPersonalDeleteExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExamResult"], "Indekser: " + deleteExamResult.ExamResultIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExamResult);
                }
                #endregion

                return RedirectToAction("DisplayAllUsers", "Users", new { message = "Usunięto wskazanego użytkownika systemu" });
            }

            return View("DeleteEntity", userToDelete);
        }

        // GET: WorkerCompetences
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerCompetences()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
            var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

            List<DisplayGivenCertificateToUserViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

            if (givenCertificates.Count != 0)
            {
                foreach (var givenCertificate in givenCertificates)
                {
                    var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;
                    singleGivenCertificate.Course = courseViewModel;

                    listOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (givenDegrees.Count != 0)
            {
                foreach (var givenDegree in givenDegrees)
                {
                    var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    listOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            WorkerCompetencesViewModel workerCompetences = new WorkerCompetencesViewModel();

            workerCompetences.UserIdentificator = user.Id;
            workerCompetences.GivenCertificates = listOfGivenCertificates;
            workerCompetences.GivenDegrees = listOfGivenDegrees;

            return View(workerCompetences);
        }

        #region AjaxQuery
        // GET: GetUsersNotEnrolledInCourse
        [Authorize(Roles = "Admin, Examiner")]
        public string[][] GetUsersNotEnrolledInCourse(string courseIdentificator)
        {
            var users = _context.userRepository.GetListOfWorkers();
            var notEnrolledInCourseUsers = users.Where(z => !z.Courses.Contains(courseIdentificator)).ToList();

            string[][] usersArray = new string[notEnrolledInCourseUsers.Count()][];

            for (int i = 0; i < notEnrolledInCourseUsers.Count(); i++)
            {
                usersArray[i] = new string[2];

                usersArray[i][0] = notEnrolledInCourseUsers[i].Id;
                usersArray[i][1] = notEnrolledInCourseUsers[i].FirstName + " " + notEnrolledInCourseUsers[i].LastName + " | " + notEnrolledInCourseUsers[i].Email;
            }

            return usersArray;
        }
        #endregion  
    }
}
