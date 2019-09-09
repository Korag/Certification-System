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

        public UsersController(
            UserManager<CertificationPlatformUser> userManager,
            RoleManager<MongoRole> roleManager,
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _emailSender = emailSender;
        }

        // GET: DisplayAllUsers
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllUsers()
        {
            var Users = _context.userRepository.GetListOfUsers();
            List<DisplayUserViewModel> usersToDisplay = new List<DisplayUserViewModel>();

            ViewBag.AvailableRoleFilters = _context.userRepository.GetAvailableRoleFiltersAsSelectList();

            foreach (var user in Users)
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

                var User = _context.userRepository.GetUserById(userIdentificator);

                DisplayAllUserInformationViewModel modifiedUser = _mapper.Map<DisplayAllUserInformationViewModel>(User);

                modifiedUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker).Select(z => z.CompanyName).ToList();
                modifiedUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager).Select(z => z.CompanyName).ToList();

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
            var User = _context.userRepository.GetUserById(userIdentificator);

            EditUserViewModel userToUpdate = _mapper.Map<EditUserViewModel>(User);
            userToUpdate.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();
            userToUpdate.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();

            return View(userToUpdate);
        }

        // POST: EditUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel editedUser)
        {
            if (ModelState.IsValid)
            {
                var OriginUser = _userManager.FindByIdAsync(editedUser.UserIdentificator).Result;
                _userManager.RemoveFromRolesAsync(OriginUser, _userManager.GetRolesAsync(OriginUser).Result.ToArray()).Wait();

                if (editedUser.SelectedRole.FirstOrDefault() != "Instructor&Examiner")
                {
                _userManager.AddToRolesAsync(OriginUser, editedUser.SelectedRole).Wait();
                }
                else
                {
                    _userManager.AddToRoleAsync(OriginUser, "Instructor").Wait();
                    _userManager.AddToRoleAsync(OriginUser, "Examiner").Wait();
                }

                OriginUser = _mapper.Map<EditUserViewModel, CertificationPlatformUser>(editedUser, OriginUser);

                _context.userRepository.UpdateUser(OriginUser);

                return RedirectToAction("ConfirmationOfActionOnUser", "Users", new { userIdentificator = OriginUser.Id, TypeOfAction = "Update" });
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

            var User = _context.userRepository.GetUserById(userIdentificator);
            var GivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(User.GivenCertificates);
            var Courses = _context.courseRepository.GetCoursesById(User.Courses);
            var GivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(User.GivenDegrees);

            var CompaniesRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker);
            var CompaniesRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager);

            List<Company> Companies = CompaniesRoleWorker.ToList();

            foreach (var company in CompaniesRoleManager)
            {
                if (Companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    Companies.Add(company);
                }
            }

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                foreach (var course in Courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCourses.Add(singleCourse);
                }
            }

            List<DisplayGivenCertificateToUserViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

            if (GivenCertificates.Count != 0)
            {
                foreach (var givenCertificate in GivenCertificates)
                {
                    var Course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                    DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;
                    singleGivenCertificate.Course = courseViewModel;

                    ListOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> ListOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (GivenDegrees.Count != 0)
            {
                foreach (var givenDegree in GivenDegrees)
                {
                    var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    ListOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            //todo: translate user roles
            UserDetailsViewModel UserDetails = _mapper.Map<UserDetailsViewModel>(User);
            UserDetails.GivenCertificates = ListOfGivenCertificates;
            UserDetails.GivenDegrees = ListOfGivenDegrees;
            UserDetails.Courses = ListOfCourses;
            UserDetails.Companies = ListOfCompanies;

            return View(UserDetails);
        }


        // GET: AnonymouslyVerificationOfUser
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfUser(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);
            var GivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(User.GivenCertificates);
            var GivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(User.GivenDegrees);

            var CompaniesRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker);
            var CompaniesRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager);

            List<Company> Companies = CompaniesRoleWorker.ToList();

            foreach (var company in CompaniesRoleManager)
            {
                if (Companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    Companies.Add(company);
                }
            }

            List<DisplayGivenCertificateToUserWithoutCourseViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();

            if (GivenCertificates.Count != 0)
            {
                foreach (var givenCertificate in GivenCertificates)
                {
                    var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                    DisplayGivenCertificateToUserWithoutCourseViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;

                    ListOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> ListOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (GivenDegrees.Count != 0)
            {
                foreach (var givenDegree in GivenDegrees)
                {
                    var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    ListOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            UserDetailsForAnonymousViewModel VerifiedUser = _mapper.Map<UserDetailsForAnonymousViewModel>(User);
            VerifiedUser.GivenCertificates = ListOfGivenCertificates;
            VerifiedUser.GivenDegrees = ListOfGivenDegrees;
            VerifiedUser.Companies = ListOfCompanies;

            return View(VerifiedUser);
        }

        // GET: InstructorDetails
        [Authorize(Roles = "Admin")]
        public ActionResult InstructorDetails(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);

            var Meetings = _context.meetingRepository.GetMeetingsByInstructorId(userIdentificator);
            var Courses = _context.courseRepository.GetCoursesByMeetingsId(Meetings.Select(z => z.MeetingIdentificator).ToList());

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                foreach (var course in Courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCourses.Add(singleCourse);
                }
            }

            List<DisplayMeetingWithoutInstructorViewModel> ListOfMeetings = new List<DisplayMeetingWithoutInstructorViewModel>();

            if (Meetings.Count != 0)
            {
                foreach (var meeting in Meetings)
                {
                    DisplayMeetingWithoutInstructorViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutInstructorViewModel>(meeting);
                    singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Courses.Where(z => z.Meetings.Contains(meeting.MeetingIdentificator)).FirstOrDefault());

                    ListOfMeetings.Add(singleMeeting);
                }
            }

            InstructorDetailsViewModel InstructorDetails = _mapper.Map<InstructorDetailsViewModel>(User);
            InstructorDetails.Courses = ListOfCourses;
            InstructorDetails.Meetings = ListOfMeetings;

            return View(InstructorDetails);
        }

        // GET: AccountDetails
        [Authorize]
        public ActionResult AccountDetails(string userIdentificator)
        {
            CertificationPlatformUser User;

            if (userIdentificator == null)
            {
                User = _userManager.FindByEmailAsync(this.User.Identity.Name).Result;
            }
            else
            {
                User = _context.userRepository.GetUserById(userIdentificator);
            }

            AccountDetailsViewModel AccountDetails = _mapper.Map<AccountDetailsViewModel>(User);

            return View(AccountDetails);
        }

        // GET: CompanyWithAccountDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CompanyWithAccountDetails(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);

            var Company = _context.companyRepository.GetCompanyById(User.CompanyRoleManager.FirstOrDefault());
            var UsersConnectedToCompany = _context.userRepository.GetUsersConnectedToCompany(Company.CompanyIdentificator);

            List<DisplayUserViewModel> ListOfUsers = new List<DisplayUserViewModel>();

            if (UsersConnectedToCompany.Count != 0)
            {
                ListOfUsers = _mapper.Map<List<DisplayUserViewModel>>(UsersConnectedToCompany);

                ListOfUsers.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).ToList().Select(s => s.CompanyName).ToList());
                ListOfUsers.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).ToList().Select(s => s.CompanyName).ToList());
            }

            CompanyWithAccountDetailsViewModel companyWithAccountDetails = _mapper.Map<CompanyWithAccountDetailsViewModel>(Company);
            companyWithAccountDetails.UsersConnectedToCompany = ListOfUsers;
            companyWithAccountDetails.UserAccount = _mapper.Map<AccountDetailsViewModel>(User);

            return View(companyWithAccountDetails);
        }

        // GET: EditAccount
        [Authorize]
        public ActionResult EditAccount(string userIdentificator, string message)
        {
            ViewBag.message = message;

            CertificationPlatformUser User = _context.userRepository.GetUserById(userIdentificator);

            EditAccountViewModel AccountDetails = _mapper.Map<EditAccountViewModel>(User);

            return View(AccountDetails);
        }

        // POST: EditAccount
        [Authorize]
        [HttpPost]
        public ActionResult EditAccount(EditAccountViewModel editedAccount)
        {
            if (ModelState.IsValid)
            {
                var OriginUser = _userManager.FindByIdAsync(editedAccount.UserIdentificator).Result;

                OriginUser = _mapper.Map<EditAccountViewModel, CertificationPlatformUser>(editedAccount, OriginUser);

                _context.userRepository.UpdateUser(OriginUser);

                return RedirectToAction("EditAccount", "Users", new { userIdentificator = OriginUser.Id, message = "Pomyślnie zaktualizowano dane" });
            }

            return View(editedAccount);
        }

        // GET: ExaminerDetails
        [Authorize(Roles = "Admin")]
        public ActionResult ExaminerDetails(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);

            var Exams = _context.examRepository.GetExamsByExaminerId(userIdentificator);
            var ExamsTerms = _context.examTermRepository.GetExamTermsByExaminerId(userIdentificator);

            var Courses = _context.courseRepository.GetExaminerCourses(userIdentificator, Exams);

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                foreach (var course in Courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCourses.Add(singleCourse);
                }
            }

            List<DisplayExamWithoutExaminerViewModel> ListOfExams = new List<DisplayExamWithoutExaminerViewModel>();

            if (Exams.Count != 0)
            {
                foreach (var exam in Exams)
                {
                    DisplayExamWithoutExaminerViewModel singleExam = _mapper.Map<DisplayExamWithoutExaminerViewModel>(exam);
                    singleExam.UsersQuantitiy = exam.EnrolledUsers.Count();
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Courses.ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)));


                    ListOfExams.Add(singleExam);
                }
            }

            List<DisplayExamTermWithoutExaminerViewModel> ListOfExamsTerms = new List<DisplayExamTermWithoutExaminerViewModel>();

            if (ExamsTerms.Count != 0)
            {
                foreach (var examTerm in ExamsTerms)
                {
                    DisplayExamTermWithoutExaminerViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExaminerViewModel>(examTerm);
                    singleExamTerm.UsersQuantitiy = examTerm.EnrolledUsers.Count();
                    singleExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(Exams.Where(z => z.ExamDividedToTerms == true && z.ExamTerms.Contains(examTerm.ExamTermIdentificator)).FirstOrDefault());

                    ListOfExamsTerms.Add(singleExamTerm);
                }
            }

            ExaminerDetailsViewModel ExaminerDetails = _mapper.Map<ExaminerDetailsViewModel>(User);
            ExaminerDetails.Courses = ListOfCourses;
            ExaminerDetails.Exams = ListOfExams;
            ExaminerDetails.ExamsTerms = ListOfExamsTerms;

            return View(ExaminerDetails);
        }

        // GET: InstructorExaminerDetails
        [Authorize(Roles = "Admin")]
        public ActionResult InstructorExaminerDetails(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);

            var Exams = _context.examRepository.GetExamsByExaminerId(userIdentificator);
            var ExamsTerms = _context.examTermRepository.GetExamTermsByExaminerId(userIdentificator);

            var Meetings = _context.meetingRepository.GetMeetingsByInstructorId(userIdentificator);
            var CoursesInstructor = _context.courseRepository.GetCoursesByMeetingsId(Meetings.Select(z => z.MeetingIdentificator).ToList());
            var CoursesExaminer = _context.courseRepository.GetExaminerCourses(userIdentificator, Exams);

            List<DisplayCourseViewModel> ListOfCoursesInstructor = new List<DisplayCourseViewModel>();

            if (CoursesInstructor.Count != 0)
            {
                foreach (var course in CoursesInstructor)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCoursesInstructor.Add(singleCourse);
                }
            }

            List<DisplayCourseViewModel> ListOfCoursesExaminer = new List<DisplayCourseViewModel>();

            if (CoursesExaminer.Count != 0)
            {
                foreach (var course in CoursesExaminer)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCoursesExaminer.Add(singleCourse);
                }
            }

            List<DisplayMeetingWithoutInstructorViewModel> ListOfMeetings = new List<DisplayMeetingWithoutInstructorViewModel>();

            if (Meetings.Count != 0)
            {
                foreach (var meeting in Meetings)
                {
                    DisplayMeetingWithoutInstructorViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutInstructorViewModel>(meeting);
                    singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(CoursesInstructor.Where(z => z.Meetings.Contains(meeting.MeetingIdentificator)).FirstOrDefault());

                    ListOfMeetings.Add(singleMeeting);
                }
            }

            List<DisplayExamWithoutExaminerViewModel> ListOfExams = new List<DisplayExamWithoutExaminerViewModel>();

            if (Exams.Count != 0)
            {
                foreach (var exam in Exams)
                {
                    DisplayExamWithoutExaminerViewModel singleExam = _mapper.Map<DisplayExamWithoutExaminerViewModel>(exam);
                    singleExam.UsersQuantitiy = exam.EnrolledUsers.Count();
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(CoursesExaminer.ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)).FirstOrDefault());


                    ListOfExams.Add(singleExam);
                }
            }

            List<DisplayExamTermWithoutExaminerViewModel> ListOfExamsTerms = new List<DisplayExamTermWithoutExaminerViewModel>();

            if (ExamsTerms.Count != 0)
            {
                foreach (var examTerm in ExamsTerms)
                {
                    DisplayExamTermWithoutExaminerViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExaminerViewModel>(examTerm);
                    singleExamTerm.UsersQuantitiy = examTerm.EnrolledUsers.Count();

                    ListOfExamsTerms.Add(singleExamTerm);
                }
            }

            InstructorExaminerDetailsViewModel ExaminerDetails = _mapper.Map<InstructorExaminerDetailsViewModel>(User);
            ExaminerDetails.CoursesInstructor = ListOfCoursesInstructor;
            ExaminerDetails.CoursesExaminer = ListOfCoursesExaminer;
            ExaminerDetails.Meetings = ListOfMeetings;

            ExaminerDetails.Exams = ListOfExams;
            ExaminerDetails.ExamsTerms = ListOfExamsTerms;

            return View(ExaminerDetails);
        }
    }
}
