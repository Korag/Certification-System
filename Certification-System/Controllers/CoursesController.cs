using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using System;
using Certification_System.Extensions;

namespace Certification_System.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public CoursesController(
            MongoOperations context, 
            IMapper mapper, 
            IKeyGenerator keyGenerator,
            ILogService logger,
            IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: ConfirmationOfActionOnCourse
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCourse(string courseIdentificator, string TypeOfAction)
        {
            if (courseIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var course = _context.courseRepository.GetCourseById(courseIdentificator);

                DisplayCourseWithMeetingsViewModel modifiedCourse = new DisplayCourseWithMeetingsViewModel();
                modifiedCourse.Course = _mapper.Map<DisplayCourseViewModel>(course);

                if (course.Meetings != null)
                {
                    var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);
                    modifiedCourse.Meetings = _mapper.Map<List<DisplayMeetingWithoutCourseViewModel>>(meetings);

                    foreach (var meeting in modifiedCourse.Meetings)
                    {
                        var instructors = _context.userRepository.GetUsersById(meetings.Where(z => z.MeetingIdentificator == meeting.MeetingIdentificator).Select(z => z.Instructors).FirstOrDefault());

                        meeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors);
                    }
                }

                var branchNames = _context.branchRepository.GetBranchesById(course.Branches);
                modifiedCourse.Course.Branches = branchNames;

                return View(modifiedCourse);
            }

            return RedirectToAction(nameof(AddCourseMenu));
        }

        // GET: AddCourseMenu
        [Authorize(Roles = "Admin")]
        public ActionResult AddCourseMenu()
        {
            return View();
        }

        // POST: AddCourseMenu
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddCourseMenu(AddCourseTypeOfActionViewModel addCourseTypeOfAction)
        {
            if (addCourseTypeOfAction.MeetingsQuantity != 0)
            {
                return RedirectToAction("AddNewCourseWithMeetings", "Courses", new { quantityOfMeetings = addCourseTypeOfAction.MeetingsQuantity });
            }
            else
            {
                return RedirectToAction("AddNewCourse", "Courses");
            }
        }

        // GET: AddNewCourse
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCourse()
        {
            AddCourseViewModel newCourse = new AddCourseViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList()
            };

            return View(newCourse);
        }

        // POST: AddNewCourse
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCourse(AddCourseViewModel newCourse)
        {
            if (ModelState.IsValid)
            {
                Course course = _mapper.Map<Course>(newCourse);
                course.CourseIdentificator = _keyGenerator.GenerateNewId();
                course.CourseIndexer = _keyGenerator.GenerateCourseEntityIndexer(course.Name);

                if (course.DateOfEnd != null)
                {
                    course.CourseLength = course.DateOfEnd.Subtract(course.DateOfStart).Days;
                }

                _context.courseRepository.AddCourse(course);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCourseLog(course, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCourse", new { courseIdentificator = course.CourseIdentificator, TypeOfAction = "Update" });
            }

            newCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            return View(newCourse);
        }

        // GET: AddNewCourseWithMeetings
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCourseWithMeetings(int quantityOfMeetings)
        {
            AddCourseWithMeetingsViewModel newCourse = new AddCourseWithMeetingsViewModel
            {
                AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList(),
                AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList(),
                Meetings = new List<AddMeetingWithoutCourseViewModel>()
            };

            for (int i = 0; i < quantityOfMeetings; i++)
            {
                newCourse.Meetings.Add(new AddMeetingWithoutCourseViewModel());
            };

            return View(newCourse);
        }

        // POST: AddNewCourseWithMeetings
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCourseWithMeetings(AddCourseWithMeetingsViewModel newCourse)
        {
            if (ModelState.IsValid)
            {
                Course course = _mapper.Map<Course>(newCourse);
                course.CourseIdentificator = _keyGenerator.GenerateNewId();
                course.CourseIndexer = _keyGenerator.GenerateCourseEntityIndexer(course.Name);

                if (course.DateOfEnd != null)
                {
                    course.CourseLength = course.DateOfEnd.Subtract(course.DateOfStart).Days;
                }

                List<Meeting> meetings = new List<Meeting>();

                if (newCourse.Meetings.Count() != 0)
                {
                    foreach (var newMeeting in newCourse.Meetings)
                    {
                        Meeting meeting = _mapper.Map<Meeting>(newMeeting);
                        meeting.MeetingIdentificator = _keyGenerator.GenerateNewId();
                        meeting.MeetingIndexer = _keyGenerator.GenerateMeetingEntityIndexer(course.CourseIndexer);

                        meetings.Add(meeting);
                    }

                    _context.meetingRepository.AddMeetings(meetings);

                    var logInfoMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                    _logger.AddMeetingsLogs(meetings, logInfoMeetings);
                }

                course.Meetings = meetings.Select(z => z.MeetingIdentificator).ToList();

                _context.courseRepository.AddCourse(course);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddCourseLog(course, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCourse", new { courseIdentificator = course.CourseIdentificator, TypeOfAction = "Add" });
            }

            newCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            newCourse.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();

            return View(newCourse);
        }

        // GET: DisplayAllCourses
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCourses(string message = null)
        {
            ViewBag.Message = message;

            var courses = _context.courseRepository.GetListOfCourses();
            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(courses);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                listOfCourses.ForEach(z => z.EnrolledUsersQuantity = courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(listOfCourses);
        }

        // GET: CourseDetails
        [Authorize(Roles = "Admin, Instructor, Examiner")]
        public ActionResult CourseDetails(string courseIdentificator, string message)
        {
            ViewBag.Message = message;

            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

            // przemyśleć nad zmniejszeniem ViewModelu -> niepotrzebne pola
            foreach (var meeting in meetings)
            {
                var meetingInstructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingWithoutCourseViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(meetingInstructors);

                meetingsViewModel.Add(singleMeeting);
            }

            var users = _context.userRepository.GetUsersById(course.EnrolledUsers);

            List<DisplayCrucialDataUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(users);
            //usersViewModel.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).Select(s => s.CompanyName).ToList());
            //usersViewModel.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).Select(s => s.CompanyName).ToList());

            var instructorsIdentificators = new List<string>();
            meetings.ToList().ForEach(z => z.Instructors.ToList().ForEach(s => instructorsIdentificators.Add(s)));

            var instructors = _context.userRepository.GetUsersById(instructorsIdentificators.Distinct().ToList());
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(instructors);

            var exams = _context.examRepository.GetExamsById(course.Exams);
            List<DisplayExamWithoutCourseViewModel> examsViewModel = new List<DisplayExamWithoutCourseViewModel>();

            List<string> listOfExaminatorsIdentificators = new List<string>();

            foreach (var exam in exams)
            {
                DisplayExamWithoutCourseViewModel singleExam = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners));

                singleExam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).Days;
                singleExam.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).Minutes;

                var examTerms= _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                examTerms.ToList().ForEach(z => listOfExaminatorsIdentificators.AddRange(z.Examiners));

                listOfExaminatorsIdentificators.AddRange(exam.Examiners);

                examsViewModel.Add(singleExam);
            }

            listOfExaminatorsIdentificators.Distinct();

            var examiners = _context.userRepository.GetUsersById(listOfExaminatorsIdentificators);
            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(examiners);

            CourseDetailsViewModel courseDetails = new CourseDetailsViewModel();
            courseDetails.Course = _mapper.Map<DisplayCourseViewModel>(course);
            courseDetails.Course.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.Exams = examsViewModel;

            courseDetails.Course.EnrolledUsersQuantity = course.EnrolledUsers.Count;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;
            courseDetails.Examiners = examinersViewModel;

            // check and delete

            //courseDetails.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(usersViewModel);

            //if (Course.CourseEnded)
            //{
            //    foreach (var user in Users)
            //    {
            //        var UsersGivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

            //        if (UsersGivenCertificates.Where(z => z.Course == Course.CourseIdentificator).Count() != 0)
            //        {
            //            courseDetails.DispensedGivenCertificates.Where(z => z.UserIdentificator == user.Id).Select(z => z.GivenCertificateIsEarned = true);
            //        }
            //    }
            //}

            if (this.User.IsInRole("Instructor") && this.User.IsInRole("Examiner"))
            {
                return View("InstructorExaminerCourseDetails", courseDetails);
            }
            else if (this.User.IsInRole("Instructor"))
            {
                return View("InstructorCourseDetails", courseDetails);
            }
            else if (this.User.IsInRole("Examiner"))
            {
                return View("ExaminerCourseDetails", courseDetails);
            }

            return View(courseDetails);
        }

        // GET: EditCourseHub
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourseHub(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);

            if (course.Meetings.Count() != 0)
            {
                return RedirectToAction("EditCourseWithMeetings", "Courses", new { courseIdentificator = courseIdentificator });
            }
            else
            {
                return RedirectToAction("EditCourse", "Courses", new { courseIdentificator = courseIdentificator });
            }
        }

        // GET: EditCourse
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);

            EditCourseViewModel courseToUpdate = _mapper.Map<EditCourseViewModel>(course);
            courseToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            return View(courseToUpdate);
        }

        // POST: EditCourse
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCourse(EditCourseViewModel editedCourse)
        {
            var originCourse = _context.courseRepository.GetCourseById(editedCourse.CourseIdentificator);

            if (ModelState.IsValid)
            {
                originCourse = _mapper.Map<EditCourseViewModel, Course>(editedCourse, originCourse);

                if (originCourse.DateOfEnd != null)
                {
                    originCourse.CourseLength = originCourse.DateOfEnd.Subtract(originCourse.DateOfStart).Days;
                }

                _context.courseRepository.UpdateCourse(originCourse);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(originCourse, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCourse", "Courses", new { courseIdentificator = editedCourse.CourseIdentificator, TypeOfAction = "Update" });
            }

            editedCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (editedCourse.SelectedBranches == null)
            {
                editedCourse.SelectedBranches = new List<string>();
            }

            return View(editedCourse);
        }

        // GET: EditCourseWithMeetings
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourseWithMeetings(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            EditCourseWithMeetingsViewModel courseToUpdate = _mapper.Map<EditCourseWithMeetingsViewModel>(course);
            courseToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            courseToUpdate.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();
            courseToUpdate.Meetings = _mapper.Map<List<EditMeetingViewModel>>(meetings);

            return View(courseToUpdate);
        }

        // POST: EditCourseWithMeetings
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCourseWithMeetings(EditCourseWithMeetingsViewModel editedCourse)
        {
            var originCourse = _context.courseRepository.GetCourseById(editedCourse.CourseIdentificator);
            var originMeetings = _context.meetingRepository.GetMeetingsById(editedCourse.Meetings.Select(z => z.MeetingIdentificator).ToList());

            if (ModelState.IsValid)
            {
                originMeetings = _mapper.Map<IList<EditMeetingViewModel>, ICollection<Meeting>>(editedCourse.Meetings, originMeetings);

                _context.meetingRepository.UpdateMeetings(originMeetings);

                var logInfoMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddMeetingsLogs(originMeetings, logInfoMeetings);

                originCourse = _mapper.Map<EditCourseWithMeetingsViewModel, Course>(editedCourse, originCourse);

                if (originCourse.DateOfEnd != null)
                {
                    originCourse.CourseLength = originCourse.DateOfEnd.Subtract(originCourse.DateOfStart).Days;
                }

                _context.courseRepository.UpdateCourse(originCourse);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(originCourse, logInfo);

                return RedirectToAction("ConfirmationOfActionOnCourse", "Courses", new { courseIdentificator = editedCourse.CourseIdentificator, TypeOfAction = "Update" });
            }

            editedCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            editedCourse.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();

            return View(editedCourse);
        }

        // GET: AssignUserToCourse
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToCourse(ICollection<string> userIdentificators, string courseIdentificator)
        {
            ICollection<string> chosenUsers;
            string chosenCourse = "";

            if (userIdentificators.Count() != 0)
                chosenUsers = _context.userRepository.GetUsersById(userIdentificators).Select(z => z.Id).ToList();
            else
                chosenUsers = new List<string>();

            if (courseIdentificator != null)
                chosenCourse = _context.courseRepository.GetCourseById(courseIdentificator).CourseIdentificator;

            var availableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            var availableCourses = _context.courseRepository.GetActiveCoursesWithVacantSeatsAsSelectList().ToList();

            AssignUserToCourseViewModel usersToAssignToCourse = new AssignUserToCourseViewModel
            {
                AvailableCourses = availableCourses,
                SelectedCourse = chosenCourse,

                AvailableUsers = availableUsers,
                SelectedUsers = chosenUsers
            };

            return View(usersToAssignToCourse);
        }

        // POST: AssignUserToCourse
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AssignUserToCourse(AssignUserToCourseViewModel usersAssignedToCourse)
        {
            if (ModelState.IsValid)
            {
                var usersAlreadyInChosenCourse = _context.userRepository.GetUsersById(usersAssignedToCourse.SelectedUsers.ToList()).Where(z => z.Courses.Contains(usersAssignedToCourse.SelectedCourse)).ToList();

                if (usersAlreadyInChosenCourse.Count() == 0)
                {
                    var course = _context.courseRepository.GetCourseById(usersAssignedToCourse.SelectedCourse);
                    var vacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

                    if (vacantSeats < usersAssignedToCourse.SelectedUsers.Count())
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranych użytkowników");
                        ModelState.AddModelError("", $"Do wybranego kursu maksymalnie możesz zapisać jeszcze: {vacantSeats} użytkowników");
                    }
                    else
                    {
                        _context.userRepository.AddUsersToCourse(course.CourseIdentificator, usersAssignedToCourse.SelectedUsers);
                        _context.courseRepository.AddEnrolledUsersToCourse(course.CourseIdentificator, usersAssignedToCourse.SelectedUsers);

                        var updatedCourse = _context.courseRepository.GetCourseById(course.CourseIdentificator);
                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddCourseLog(updatedCourse, logInfo);

                        var updatedUsers= _context.userRepository.GetUsersById(usersAssignedToCourse.SelectedUsers);
                        _logger.AddUsersLogs(updatedUsers, logInfo);

                        return RedirectToAction("CourseDetails", new { courseIdentificator = usersAssignedToCourse.SelectedCourse, message = "Zapisano nowych użytkowników na kurs" });
                    }
                }
                else
                {
                    foreach (var user in usersAlreadyInChosenCourse)
                    {
                        ModelState.AddModelError("", user.FirstName + " " + user.LastName + " już jest zapisany/a do wybranego kursu");
                    }
                }
            }

            usersAssignedToCourse.AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            usersAssignedToCourse.AvailableCourses = _context.courseRepository.GetActiveCoursesWithVacantSeatsAsSelectList().ToList();

            return View(usersAssignedToCourse);
        }

        // GET: EndCourseAndDispenseGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult EndCourseAndDispenseGivenCertificates(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            var exams = _context.examRepository.GetExamsById(course.Exams);

            if (course.CourseEnded != true)
            {
                var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);

                List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

                if (enrolledUsersList.Count != 0)
                {
                    listOfUsers = GetCourseListOfUsersWithStatistics(enrolledUsersList, meetings, exams).ToList();
                }

                DispenseGivenCertificatesViewModel courseToEndViewModel = _mapper.Map<DispenseGivenCertificatesViewModel>(course);
                courseToEndViewModel.AllCourseParticipants = listOfUsers;
                courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

                courseToEndViewModel.CourseLength = courseToEndViewModel.DateOfEnd.Subtract(courseToEndViewModel.DateOfStart).Days;
                courseToEndViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);
                courseToEndViewModel.EnrolledUsersQuantity = course.EnrolledUsers.Count;

                courseToEndViewModel.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(listOfUsers);

                return View(courseToEndViewModel);
            }

            return RedirectToAction("CourseDetails", new { courseIdentificator = courseIdentificator });
        }

        // POST: EndCourseAndDispenseGivenCertificates
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EndCourseAndDispenseGivenCertificates(DispenseGivenCertificatesViewModel courseToEndViewModel)
        {
            var course = _context.courseRepository.GetCourseById(courseToEndViewModel.CourseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            if (ModelState.IsValid)
            {
                course.CourseEnded = true;

                var certificate = _context.certificateRepository.GetCertificateById(courseToEndViewModel.SelectedCertificate);

                _context.courseRepository.UpdateCourse(course);

                List<GivenCertificate> disposedGivenCertificates = new List<GivenCertificate>();
                List<CertificationPlatformUser> updatedUsers = new List<CertificationPlatformUser>();

                for (int i = 0; i < courseToEndViewModel.DispensedGivenCertificates.Count(); i++)
                {
                    if (courseToEndViewModel.DispensedGivenCertificates[i].GivenCertificateIsEarned == true)
                    {
                        GivenCertificate singleGivenCertificate = new GivenCertificate
                        {
                            GivenCertificateIdentificator = _keyGenerator.GenerateNewId(),
                            GivenCertificateIndexer = _keyGenerator.GenerateGivenCertificateEntityIndexer(certificate.CertificateIndexer),

                            ReceiptDate = courseToEndViewModel.ReceiptDate,
                            ExpirationDate = courseToEndViewModel.ExpirationDate,

                            Depreciated = false,
                            Course = courseToEndViewModel.CourseIdentificator,
                            Certificate = courseToEndViewModel.SelectedCertificate
                        };

                        disposedGivenCertificates.Add(singleGivenCertificate);
                        _context.givenCertificateRepository.AddGivenCertificate(singleGivenCertificate);

                        var user = _context.userRepository.GetUserById(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator);
                        user.GivenCertificates.Add(singleGivenCertificate.GivenCertificateIdentificator);

                        updatedUsers.Add(user);
                        _context.userRepository.UpdateUser(user);
                    }
                }

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddUsersLogs(updatedUsers, logInfo);
                _logger.AddGivenCertificatesLogs(disposedGivenCertificates, logInfo);

                return RedirectToAction("CourseDetails", new { courseIdentificator = courseToEndViewModel.CourseIdentificator, message = "Zaknięto kurs i rozdano certyfikaty." });
            }

            var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);
            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            var exams = _context.examRepository.GetExamsById(course.Exams);

            List<string> AllExamsResultsIdentificators = new List<string>();
            exams.ToList().ForEach(z => AllExamsResultsIdentificators.AddRange(z.ExamResults));

            var allExamsResults = _context.examResultRepository.GetExamsResultsById(AllExamsResultsIdentificators);

            if (enrolledUsersList.Count != 0)
            {
                listOfUsers = GetCourseListOfUsersWithStatistics(enrolledUsersList, meetings, exams).ToList();
            }

            courseToEndViewModel.AllCourseParticipants = listOfUsers;
            courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

            return View(courseToEndViewModel);
        }

        // GET: DisplayCourseSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayCourseSummary(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            var exams = _context.examRepository.GetExamsById(course.Exams);

            var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);

            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            if (enrolledUsersList.Count != 0)
            {
                listOfUsers = GetCourseListOfUsersWithAllStatistics(enrolledUsersList, meetings, exams).ToList();
            }

            DisplayCourseSummaryViewModel courseSummaryViewModel = _mapper.Map<DisplayCourseSummaryViewModel>(course);
            courseSummaryViewModel.AllCourseParticipants = listOfUsers;
            courseSummaryViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseSummaryViewModel.AllCourseExams = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(exams.OrderBy(z=> z.ExamIndexer));
            courseSummaryViewModel.CourseLength = courseSummaryViewModel.DateOfEnd.Subtract(courseSummaryViewModel.DateOfStart).Days;
            courseSummaryViewModel.EnrolledUsersQuantity = course.EnrolledUsers.Count;

            return View(courseSummaryViewModel);
        }

        // GET: DeleteUserFromCourse
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromCourse(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);

            if (course.CourseEnded != true)
            {
                var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (enrolledUsersList.Count != 0)
                {
                    ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledUsersList);
                }

                DeleteUsersFromCourseViewModel deleteUsersFromCourseViewModel = _mapper.Map<DeleteUsersFromCourseViewModel>(course);
                deleteUsersFromCourseViewModel.AllCourseParticipants = ListOfUsers;

                deleteUsersFromCourseViewModel.CourseLength = deleteUsersFromCourseViewModel.DateOfEnd.Subtract(deleteUsersFromCourseViewModel.DateOfStart).Days;
                deleteUsersFromCourseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                deleteUsersFromCourseViewModel.UsersToDeleteFromCourse = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(ListOfUsers);

                return View(deleteUsersFromCourseViewModel);
            }

            return RedirectToAction("CourseDetails", new { courseIdentificator = courseIdentificator });
        }

        // POST: DeleteUserFromCourse
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromCourse(DeleteUsersFromCourseCrucialViewModel deleteUsersFromCourseViewModel)
        {
            if (ModelState.IsValid)
            {
                var usersToDeleteFromCourseIdentificators = deleteUsersFromCourseViewModel.UsersToDeleteFromCourse.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                _context.courseRepository.DeleteUsersFromCourse(deleteUsersFromCourseViewModel.CourseIdentificator, usersToDeleteFromCourseIdentificators);
                _context.userRepository.DeleteCourseFromUsersCollection(deleteUsersFromCourseViewModel.CourseIdentificator, usersToDeleteFromCourseIdentificators);

                var updatedCourse = _context.courseRepository.GetCourseById(deleteUsersFromCourseViewModel.CourseIdentificator);
                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(updatedCourse, logInfo);

                var updatedUsers = _context.userRepository.GetUsersById(usersToDeleteFromCourseIdentificators);
                _logger.AddUsersLogs(updatedUsers, logInfo);

                if (deleteUsersFromCourseViewModel.UsersToDeleteFromCourse.Count() == 1)
                {
                    return RedirectToAction("UserDetails", "Users", new { userIdentificator = deleteUsersFromCourseViewModel.UsersToDeleteFromCourse.FirstOrDefault().UserIdentificator, message = "Usunięto użytkownika z kursu" });
                }
                else
                {
                    return RedirectToAction("CourseDetails", new { courseIdentificator = deleteUsersFromCourseViewModel.CourseIdentificator, message = "Usunięto grupę użytkowników z kursu" });
                }
            }

            return RedirectToAction("DeleteUsersFromCourse", new { courseIdentificator = deleteUsersFromCourseViewModel.CourseIdentificator });
        }

        // GET: WorkerCourses
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerCourses()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var courses = _context.courseRepository.GetCoursesById(user.Courses);
            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(courses);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                listOfCourses.ForEach(z => z.EnrolledUsersQuantity = courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(listOfCourses);
        }

        // GET: InstructorCourses
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorCourses()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var meetings = _context.meetingRepository.GetMeetingsByInstructorId(user.Id);
            var courses = _context.courseRepository.GetCoursesByMeetingsId(meetings.Select(z => z.MeetingIdentificator).ToList());

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(courses);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                //ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(listOfCourses);
        }

        // GET: ExaminerCourses
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerCourses()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var exams = _context.examRepository.GetExamsByExaminerId(user.Id);
            var courses = _context.courseRepository.GetCoursesByExamsId(exams.Select(z => z.ExamIdentificator).ToList()).ToList();

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(courses);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                //ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(listOfCourses);
        }

        // GET: InstructorExaminerCourses
        [Authorize(Roles = "Instructor")]
        [Authorize(Roles = "Examiner")]
        public ActionResult InstructorExaminerCourses()
        {
            ViewBag.AvailableRoleFilters = _context.userRepository.GetAvailableCourseRoleFiltersAsSelectList();

            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var exams = _context.examRepository.GetExamsByExaminerId(user.Id);
            var coursesAsExaminer = _context.courseRepository.GetCoursesByExamsId(exams.Select(z => z.ExamIdentificator).ToList()).ToList();

            var meetings = _context.meetingRepository.GetMeetingsByInstructorId(user.Id);
            var coursesAsInstructor = _context.courseRepository.GetCoursesByMeetingsId(meetings.Select(z => z.MeetingIdentificator).ToList());

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

            List<DisplayCourseWithUserRoleViewModel> allCourses = new List<DisplayCourseWithUserRoleViewModel>();

            allCourses.AddRange(listOfCoursesAsExaminer);
            allCourses.AddRange(listOfCoursesAsInstructor);
            allCourses.AddRange(listOfCoursesAsBothRoles);

            return View(allCourses);
        }

        // GET: DeleteCourseHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCourseHub(string courseIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteCourseEntityLink(courseIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteCourse
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteCourse(string courseIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(courseIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel courseToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = courseIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie kursu"
                };

                return View("DeleteEntity", courseToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteCourse
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteCourse(DeleteEntityViewModel courseToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var course = _context.courseRepository.GetCourseById(courseToDelete.EntityIdentificator);

            if (course == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, courseToDelete.Code))
            {
                var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.courseRepository.DeleteCourse(courseToDelete.EntityIdentificator);
                _logger.AddCourseLog(course, logInfoDelete);

                var deletedMeetings = _context.meetingRepository.DeleteMeetings(course.Meetings);
                _logger.AddMeetingsLogs(deletedMeetings, logInfoDelete);

                var deletedExams = _context.examRepository.DeleteExams(course.Exams);
                _logger.AddExamsLogs(deletedExams, logInfoDelete);

                var deletedExamsTerms = _context.examTermRepository.DeleteExamsTerms(deletedExams.SelectMany(z=> z.ExamTerms).ToList());
                _logger.AddExamsTermsLogs(deletedExamsTerms, logInfoDelete);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResults(deletedExams.SelectMany(z => z.ExamResults).ToList());
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDelete);

                var deletedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificatesByCourseId(courseToDelete.EntityIdentificator);
                _logger.AddGivenCertificatesLogs(deletedGivenCertificates, logInfoDelete);

                var updatedUsers = _context.userRepository.DeleteCourseFromUsers(courseToDelete.EntityIdentificator);
                _logger.AddUsersLogs(updatedUsers, logInfoUpdate);

                return RedirectToAction("DisplayAllCourses", "Courses", new { message = "Usunięto wskazany kurs" });
            }

            return View("DeleteEntity", courseToDelete);
        }

        #region AjaxQuery
        // GET: GetCoursesByUserId
        [Authorize(Roles = "Admin")]
        public string[][] GetCoursesByUserId(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var courses = _context.courseRepository.GetCoursesById(user.Courses).ToList();

            string[][] coursesArray = new string[courses.Count()][];

            for (int i = 0; i < courses.Count(); i++)
            {
                coursesArray[i] = new string[2];

                coursesArray[i][0] = courses[i].CourseIdentificator;
                coursesArray[i][1] = courses[i].CourseIndexer + " | " + courses[i].Name;
            }


            return coursesArray;
        }
        #endregion

        #region HelpersMethod
        // GetCourseListOfUsersWithStatistics
        private ICollection<DisplayUserWithCourseResultsViewModel> GetCourseListOfUsersWithStatistics(ICollection<CertificationPlatformUser> enrolledUsers, ICollection<Meeting> meetings, ICollection<Exam> exams)
        {
            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            foreach (var user in enrolledUsers)
            {
                DisplayUserWithCourseResultsViewModel singleUser = _mapper.Map<DisplayUserWithCourseResultsViewModel>(user);
                singleUser.QuantityOfMeetings = meetings.Count();
                singleUser.QuantityOfPresenceOnMeetings = meetings.Where(z => z.AttendanceList.Contains(user.Id)).Count();

                try
                {
                    double UserPresencePercentage = ((meetings.Where(z => z.AttendanceList.Contains(singleUser.UserIdentificator)).Count() / meetings.Count()) * 100);
                    UserPresencePercentage = Math.Round(UserPresencePercentage, 2);
                    singleUser.PercentageOfUserPresenceOnMeetings = UserPresencePercentage;
                }
                catch (Exception e)
                {
                    singleUser.PercentageOfUserPresenceOnMeetings = 0.00;
                }

                var userExams = exams.Where(z => z.EnrolledUsers.Contains(user.Id)).GroupBy(z => z.ExamIndexer).ToList();

                for (int i = 0; i < userExams.Count; i++)
                {
                    var lastExamPeriod = userExams[i].ToList().OrderByDescending(z => z.OrdinalNumber).FirstOrDefault();
                    var lastUserResult = _context.examResultRepository.GetExamsResultsById(lastExamPeriod.ExamResults).ToList().Where(z => z.User == user.Id).ToList();

                    if (lastUserResult != null)
                    {
                        var singleExamResult = _mapper.Map<DisplayExamResultWithExamIdentificator>(lastUserResult);
                        singleExamResult.MaxAmountOfPointsToEarn = lastExamPeriod.MaxAmountOfPointsToEarn;
                        singleExamResult.ExamIdentificator = lastExamPeriod.ExamIdentificator;
                        singleExamResult.ExamOrdinalNumber = lastExamPeriod.OrdinalNumber;

                        singleUser.ExamsResults.Add(singleExamResult);
                    }
                }

                listOfUsers.Add(singleUser);
            }

            return listOfUsers;
        }

        // GetCourseListOfUsersWithAllStatistics
        private ICollection<DisplayUserWithCourseResultsViewModel> GetCourseListOfUsersWithAllStatistics(ICollection<CertificationPlatformUser> enrolledUsers, ICollection<Meeting> meetings, ICollection<Exam> exams)
        {
            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            foreach (var user in enrolledUsers)
            {
                DisplayUserWithCourseResultsViewModel singleUser = _mapper.Map<DisplayUserWithCourseResultsViewModel>(user);
                singleUser.QuantityOfMeetings = meetings.Count();
                singleUser.QuantityOfPresenceOnMeetings = meetings.Where(z => z.AttendanceList.Contains(user.Id)).Count();

                try
                {
                    double UserPresencePercentage = ((meetings.Where(z => z.AttendanceList.Contains(singleUser.UserIdentificator)).Count() / meetings.Count()) * 100);
                    UserPresencePercentage = Math.Round(UserPresencePercentage, 2);
                    singleUser.PercentageOfUserPresenceOnMeetings = UserPresencePercentage;
                }
                catch (Exception e)
                {
                    singleUser.PercentageOfUserPresenceOnMeetings = 0.00;
                }

                var userExams = exams.Where(z => z.EnrolledUsers.Contains(user.Id)).GroupBy(z => z.ExamIndexer).ToList();

                for (int i = 0; i < userExams.Count; i++)
                {
                    foreach (var exam in userExams[i])
                    {
                        var userResult = _context.examResultRepository.GetExamsResultsById(exam.ExamResults).ToList().Where(z => z.User == user.Id).FirstOrDefault();

                        if (userResult != null)
                        {
                            var singleExamResult = _mapper.Map<DisplayExamResultWithExamIdentificator>(userResult);
                            singleExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;
                            singleExamResult.ExamIdentificator = exam.ExamIdentificator;
                            singleExamResult.ExamOrdinalNumber = exam.OrdinalNumber;

                            singleUser.ExamsResults.Add(singleExamResult);
                        }

                        listOfUsers.Add(singleUser);
                    }
                }
            }

            return listOfUsers;
        }
        #endregion
    }
}


