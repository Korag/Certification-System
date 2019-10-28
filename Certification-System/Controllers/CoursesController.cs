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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

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
                modifiedCourse.Course = _mapper.Map<DisplayCourseWithPriceViewModel>(course);

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

                #region EntityLogs

                var logInfoAddCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addCourse"]);
                _logger.AddCourseLog(course, logInfoAddCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddCourse);

                #endregion

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
                ICollection<PersonalLogInformation> meetingsLogInfo = new List<PersonalLogInformation>();

                if (newCourse.Meetings.Count() != 0)
                {
                    foreach (var newMeeting in newCourse.Meetings)
                    {
                        Meeting meeting = _mapper.Map<Meeting>(newMeeting);
                        meeting.MeetingIdentificator = _keyGenerator.GenerateNewId();
                        meeting.MeetingIndexer = _keyGenerator.GenerateMeetingEntityIndexer(course.CourseIndexer);

                        meetings.Add(meeting);

                        meetingsLogInfo.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addMeeting"], "Indekser " + meeting.MeetingIndexer));

                        #region PersonalUserLogs

                        var logInfoPersonalAddInstructorsToMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addInstructorToMeeting"], "Indekser " + meeting.MeetingIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(meeting.Instructors, logInfoPersonalAddInstructorsToMeeting);

                        #endregion
                    }

                    _context.meetingRepository.AddMeetings(meetings);

                    #region EntityLogs

                    var logInfoAddMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addMeeting"]);
                    _logger.AddMeetingsLogs(meetings, logInfoAddMeetings);

                    #endregion

                    _context.personalLogRepository.AddPersonalUsersLogsToAdminGroup(meetingsLogInfo);
                }

                course.Meetings = meetings.Select(z => z.MeetingIdentificator).ToList();

                _context.courseRepository.AddCourse(course);

                #region EntityLogs

                var logInfoAddCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addCourse"]);
                _logger.AddCourseLog(course, logInfoAddCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddCourse);

                #endregion

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

            List<string> listOfExaminersIdentificators = new List<string>();

            foreach (var exam in exams)
            {
                DisplayExamWithoutCourseViewModel singleExam = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners));

                var examTerms= _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                examTerms.ToList().ForEach(z => listOfExaminersIdentificators.AddRange(z.Examiners));
                listOfExaminersIdentificators.AddRange(exam.Examiners);

                examsViewModel.Add(singleExam);
            }

            listOfExaminersIdentificators.Distinct();

            var examiners = _context.userRepository.GetUsersById(listOfExaminersIdentificators);
            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(examiners);

            CourseDetailsViewModel courseDetails = new CourseDetailsViewModel();
            courseDetails.Course = _mapper.Map<DisplayCourseWithPriceViewModel>(course);
            courseDetails.Course.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.Exams = examsViewModel;

            courseDetails.Course.EnrolledUsersQuantity = course.EnrolledUsers.Count;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;
            courseDetails.Examiners = examinersViewModel;

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

                #region EntityLogs

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCourse"]);
                _logger.AddCourseLog(originCourse, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalUpdateCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateCourse"], "Indekser: " + originCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateCourse);

                var logInfoPersonalUpdateUserCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserCourse"], "Indekser: " + originCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(originCourse.EnrolledUsers, logInfoPersonalUpdateUserCourse);

                #endregion

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
            var originMeetings = _context.meetingRepository.GetMeetingsById(editedCourse.Meetings.Select(z => z.MeetingIdentificator).ToList()).ToList();

            if (ModelState.IsValid)
            {
                ICollection<PersonalLogInformation> meetingLogInformation = new List<PersonalLogInformation>();
                ICollection<PersonalLogInformation> meetingUserLogInformation = new List<PersonalLogInformation>();

                for (int i = 0; i < originMeetings.Count(); i++)
                {
                    var originMeetingInstructors = originMeetings[i].Instructors;

                    originMeetings[i] = _mapper.Map<EditMeetingViewModel, Meeting>(editedCourse.Meetings[i], originMeetings[i]);

                    #region PersonalUserLogs

                    meetingLogInformation.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateMeeting"], "Indekser: " + originMeetings[i].MeetingIndexer));
                    meetingUserLogInformation.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserMeeting"], "Indekser: " + originMeetings[i].MeetingIndexer));

                    _context.personalLogRepository.AddPersonalUsersLogs(originMeetings[i].Instructors, _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserMeeting"], "Indekser: " + originMeetings[i].MeetingIndexer));

                    var addedInstructors = originMeetings[i].Instructors.Except(originMeetingInstructors).ToList();
                    var removedInstructors = originMeetingInstructors.Except(originMeetings[i].Instructors).ToList();

                    var logInfoPersonalAddInstructorsToMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addInstructorToMeeting"], "Indekser " + originMeetings[i].MeetingIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(addedInstructors, logInfoPersonalAddInstructorsToMeeting);

                    var logInfoPersonalRemoveInstructorsFromMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeInstructorFromMeeting"], "Indekser " + originMeetings[i].MeetingIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(removedInstructors, logInfoPersonalRemoveInstructorsFromMeeting);

                    #endregion
                }

                _context.meetingRepository.UpdateMeetings(originMeetings);

                originCourse = _mapper.Map<EditCourseWithMeetingsViewModel, Course>(editedCourse, originCourse);

                if (originCourse.DateOfEnd != null)
                {
                    originCourse.CourseLength = originCourse.DateOfEnd.Subtract(originCourse.DateOfStart).Days;
                }

                _context.courseRepository.UpdateCourse(originCourse);

                #region EntityLogs

                var logInfoUpdateMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateMeeting"]);
                _logger.AddMeetingsLogs(originMeetings, logInfoUpdateMeetings);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateCourse"]);
                _logger.AddCourseLog(originCourse, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                _context.personalLogRepository.AddPersonalUsersLogsToAdminGroup(meetingLogInformation);
                _context.personalLogRepository.AddPersonalUsersMultipleLogs(originCourse.EnrolledUsers, meetingUserLogInformation);

                var logInfoPersonalUpdateCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateCourse"], "Indekser: " + originCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateCourse);

                var logInfoPersonalUpdateUserCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserCourse"], "Indekser: " + originCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(originCourse.EnrolledUsers, logInfoPersonalUpdateUserCourse);

                #endregion

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

            var availableUsers = new List<SelectListItem>();
            var availableCourses = _context.courseRepository.GetActiveCoursesWithVacantSeatsAsSelectList().ToList();

            if (courseIdentificator != null)
            {
                chosenCourse = _context.courseRepository.GetCourseById(courseIdentificator).CourseIdentificator;

                var usersNotEnrolledInCourse = _context.userRepository.GetListOfWorkers().Where(z => !z.Courses.Contains(courseIdentificator)).Select(z => z.Id).ToList();
                availableUsers = _context.userRepository.GenerateSelectList(usersNotEnrolledInCourse);
            }
            else
            {
                availableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            }

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
                    var courseQueue = _context.courseRepository.GetCourseQueueById(course.CourseIdentificator);

                    var vacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

                    if (courseQueue != null)
                    {
                        vacantSeats = vacantSeats - courseQueue.AwaitingUsers.Count();
                    }

                    if (vacantSeats < usersAssignedToCourse.SelectedUsers.Count())
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranych użytkowników");
                        ModelState.AddModelError("", $"Do wybranego kursu maksymalnie możesz zapisać jeszcze: {vacantSeats} użytkowników");
                    }
                    else
                    {
                        var users = _context.userRepository.AddUsersToCourse(course.CourseIdentificator, usersAssignedToCourse.SelectedUsers);
                        course = _context.courseRepository.AddEnrolledUsersToCourse(course.CourseIdentificator, usersAssignedToCourse.SelectedUsers);

                        #region EntityLogs

                        var logInfoAddUsersToCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addUsersToCourse"]);
                        _logger.AddCourseLog(course, logInfoAddUsersToCourse);

                        var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToCourse"]);
                        _logger.AddUsersLogs(users, logInfoUpdateUsers);

                        #endregion

                        #region PersonalUserLogs

                        var logInfoPersonalCourseAddUsers = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUsersToCourse"], "Indekser: " + course.CourseIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalCourseAddUsers);

                        var logInfoPersonalAssingUserToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToCourse"], "Indekser: " + course.CourseIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(usersAssignedToCourse.SelectedUsers, logInfoPersonalAssingUserToCourse);

                        #endregion

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
            var lastExamsPeriods = exams.GroupBy(z => z.ExamIndexer).Select(z => z.OrderByDescending(s => s.OrdinalNumber).FirstOrDefault()).ToList();

            var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);

            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesByCourseId(course.CourseIdentificator);

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

            courseToEndViewModel.LastExamsPeriods = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(lastExamsPeriods.OrderBy(z => z.ExamIndexer));
            courseToEndViewModel.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(listOfUsers);

            courseToEndViewModel.GivenCertificates = _mapper.Map<List<DisplayGivenCertificateWithoutCourseViewModel>>(givenCertificates);

            for (int i = 0; i < givenCertificates.Count(); i++)
            {
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificates.ElementAt(i).Certificate);

                courseToEndViewModel.GivenCertificates.ElementAt(i).Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                var user = enrolledUsersList.Where(z => z.GivenCertificates.Contains(courseToEndViewModel.GivenCertificates.ElementAt(i).GivenCertificateIdentificator)).FirstOrDefault();

                courseToEndViewModel.GivenCertificates.ElementAt(i).User = _mapper.Map<DisplayCrucialDataUserViewModel>(user);
            }

            for (int i = 0; i < enrolledUsersList.Count(); i++)
            {
                var userGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(enrolledUsersList.ElementAt(i).GivenCertificates).Where(z => z.Course == courseToEndViewModel.CourseIdentificator /*&& z.Certificate == courseToEndViewModel.SelectedCertificate*/).FirstOrDefault();

                if (userGivenCertificate != null)
                {
                    courseToEndViewModel.DispensedGivenCertificates[i].GivenCertificateIsEarned = true;
                }
            }

            return View(courseToEndViewModel);
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

                ICollection<PersonalLogInformation> createdGivenCertificates = new List<PersonalLogInformation>();

                for (int i = 0; i < courseToEndViewModel.DispensedGivenCertificates.Count(); i++)
                {
                    if (courseToEndViewModel.DispensedGivenCertificates[i].GivenCertificateIsEarned == true)
                    {
                        var user = _context.userRepository.GetUserById(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator);
                        var usersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates).Where(z => z.Course == courseToEndViewModel.CourseIdentificator && z.Certificate == courseToEndViewModel.SelectedCertificate).FirstOrDefault();

                        if (usersGivenCertificate != null)
                        {
                            continue;
                        }

                        GivenCertificate singleGivenCertificate = new GivenCertificate
                        {
                            GivenCertificateIdentificator = _keyGenerator.GenerateNewId(),
                            GivenCertificateIndexer = _keyGenerator.GenerateGivenCertificateEntityIndexer(certificate.CertificateIndexer),

                            ReceiptDate = courseToEndViewModel.ReceiptDate,
                            ExpirationDate = courseToEndViewModel.ExpirationDate,

                            Course = courseToEndViewModel.CourseIdentificator,
                            Certificate = courseToEndViewModel.SelectedCertificate
                        };

                        disposedGivenCertificates.Add(singleGivenCertificate);
                        _context.givenCertificateRepository.AddGivenCertificate(singleGivenCertificate);

                        user.GivenCertificates.Add(singleGivenCertificate.GivenCertificateIdentificator);

                        updatedUsers.Add(user);
                        _context.userRepository.UpdateUser(user);

                        #region PersonalUserLogs

                        var logInfoPersonalAcquireGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserGivenCertificate"], "Indekser: " + singleGivenCertificate.GivenCertificateIndexer);
                        _context.personalLogRepository.AddPersonalUserLog(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator, logInfoPersonalAcquireGivenCertificate);

                        createdGivenCertificates.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addGivenCertificate"], "Indekser: " + singleGivenCertificate.GivenCertificateIndexer));

                        #endregion
                    }

                    #region PersonalUserLogs

                    var logInfoPersonalNotAcquireGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["userNotAcquireGivenCertificate"], "Indekser kursu: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator, logInfoPersonalNotAcquireGivenCertificate);

                    #endregion
                }

                var endedCourse = _context.courseRepository.GetCourseById(course.CourseIdentificator);

                #region EntityLogs

                var logInfoAddGivenCertificate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addGivenCertificate"]);
                var logInfoCourseEnded = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["closeCourse"]);
                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addGivenCertificateToUser"]);

                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);
                _logger.AddGivenCertificatesLogs(disposedGivenCertificates, logInfoAddGivenCertificate);

                _logger.AddCourseLog(endedCourse, logInfoCourseEnded);

                #endregion

                #region PersonalUserLog

                var logInfoPersonalCourseEnded = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["courseEnded"], "Indekser: " + endedCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalCourseEnded);

                _context.personalLogRepository.AddPersonalUsersLogsToAdminGroup(createdGivenCertificates);

                #endregion


                return RedirectToAction("CourseDetails", new { courseIdentificator = courseToEndViewModel.CourseIdentificator, message = "Zamknięto kurs i rozdano certyfikaty." });
            }

            var enrolledUsersList = _context.userRepository.GetUsersById(course.EnrolledUsers);
            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            var exams = _context.examRepository.GetExamsById(course.Exams);

            if (enrolledUsersList.Count != 0)
            {
                listOfUsers = GetCourseListOfUsersWithStatistics(enrolledUsersList, meetings, exams).ToList();
            }

            var lastExamsPeriods = exams.GroupBy(z => z.ExamIndexer).Select(z => z.OrderByDescending(s => s.OrdinalNumber).FirstOrDefault()).ToList();
            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesByCourseId(course.CourseIdentificator);

            for (int i = 0; i < enrolledUsersList.Count(); i++)
            {
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificates.ElementAt(i).Certificate);

                courseToEndViewModel.GivenCertificates.ElementAt(i).Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);
                courseToEndViewModel.GivenCertificates.ElementAt(i).User = _mapper.Map<DisplayCrucialDataUserViewModel>(enrolledUsersList.ElementAt(i));
            }

            for (int i = 0; i < givenCertificates.Count(); i++)
            {
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificates.ElementAt(i).Certificate);

                courseToEndViewModel.GivenCertificates.ElementAt(i).Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                var user = enrolledUsersList.Where(z => z.GivenCertificates.Contains(courseToEndViewModel.GivenCertificates.ElementAt(i).GivenCertificateIdentificator)).FirstOrDefault();

                courseToEndViewModel.GivenCertificates.ElementAt(i).User = _mapper.Map<DisplayCrucialDataUserViewModel>(user);
            }

            courseToEndViewModel.AllCourseParticipants = listOfUsers;
            courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

            courseToEndViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);
            courseToEndViewModel.LastExamsPeriods = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(lastExamsPeriods.OrderBy(z => z.ExamIndexer));

            courseToEndViewModel.GivenCertificates = _mapper.Map<List<DisplayGivenCertificateWithoutCourseViewModel>>(givenCertificates);

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

            courseSummaryViewModel.AllCourseExams = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(exams.OrderBy(z => z.ExamIndexer));
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

        // POST: DeleteUsersFromCourse
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

                #region EntityLogs

                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromCourse"]);
                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeGroupOfUsersFromCourse"]);

                _logger.AddCourseLog(updatedCourse, logInfoUpdateCourse);

                var updatedUsers = _context.userRepository.GetUsersById(usersToDeleteFromCourseIdentificators);
                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalRemoveUsersFromCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeGroupOfUsersFromCourse"], "Indekser: " + updatedCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalRemoveUsersFromCourse);

                var logInfoPersonalUserRemovedFromCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["userRemovedFromCourse"], "Indekser: " + updatedCourse.CourseIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(usersToDeleteFromCourseIdentificators, logInfoPersonalUserRemovedFromCourse);

                #endregion

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
                var logInfoDeleteCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteCourse"]);
                var logInfoDeleteMeetings = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteMeeting"]);

                var logInfoDeleteExams = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExam"]);
                var logInfoDeleteExamsTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamTerm"]);
                var logInfoDeleteExamsResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamResult"]);

                var logInfoDeleteGivenCertificates = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteGivenCertificate"]);

                var logInfoUpdateUsers = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromCourse"]);

                _context.courseRepository.DeleteCourse(courseToDelete.EntityIdentificator);

                #region EntityLogs

                _logger.AddCourseLog(course, logInfoDeleteCourse);

                var deletedMeetings = _context.meetingRepository.DeleteMeetings(course.Meetings);
                _logger.AddMeetingsLogs(deletedMeetings, logInfoDeleteMeetings);

                var deletedExams = _context.examRepository.DeleteExams(course.Exams);
                _logger.AddExamsLogs(deletedExams, logInfoDeleteExams);

                var deletedExamsTerms = _context.examTermRepository.DeleteExamsTerms(deletedExams.SelectMany(z => z.ExamTerms).ToList());
                _logger.AddExamsTermsLogs(deletedExamsTerms, logInfoDeleteExamsTerms);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResults(deletedExams.SelectMany(z => z.ExamResults).ToList());
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDeleteExamsResults);

                var deletedGivenCertificates = _context.givenCertificateRepository.DeleteGivenCertificatesByCourseId(courseToDelete.EntityIdentificator);
                _logger.AddGivenCertificatesLogs(deletedGivenCertificates, logInfoDeleteGivenCertificates);

                var updatedUsers = _context.userRepository.DeleteCourseFromUsers(courseToDelete.EntityIdentificator);
                _logger.AddUsersLogs(updatedUsers, logInfoUpdateUsers);

                #endregion

                #region PersonalLogs

                var allInstructors = deletedMeetings.SelectMany(z => z.Instructors).Distinct().ToList();
                var allExaminers = deletedExams.SelectMany(z => z.Examiners).Distinct().ToList();

                var logInfoPersonalDeleteCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteCourse"], "Indekser: " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteCourse);

                var logInfoPersonalDeleteUserCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserCourse"], "Indekser: " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(course.EnrolledUsers, logInfoPersonalDeleteUserCourse);
                _context.personalLogRepository.AddPersonalUsersLogs(allInstructors, logInfoPersonalDeleteUserCourse);
                _context.personalLogRepository.AddPersonalUsersLogs(allExaminers, logInfoPersonalDeleteUserCourse);

                foreach (var deletedMeeting in deletedMeetings)
                {
                    var logInfoPersonalDeleteMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteMeeting"], "Indekser " + deletedMeeting.MeetingIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteMeeting);

                    var logInfoPersonalDeleteUserMeeting = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserMeeting"], "Indekser " + deletedMeeting.MeetingIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(course.EnrolledUsers, logInfoPersonalDeleteUserMeeting);
                    _context.personalLogRepository.AddPersonalUsersLogs(deletedMeeting.Instructors, logInfoPersonalDeleteUserMeeting);
                }

                foreach (var deletedExam in deletedExams)
                {
                    var logInfoPersonalDeleteExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExam"], "Indekser " + deletedExam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExam);

                    var logInfoPersonalDeleteUserExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserExam"], "Indekser " + deletedExam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(deletedExam.EnrolledUsers, logInfoPersonalDeleteUserExam);
                    _context.personalLogRepository.AddPersonalUsersLogs(deletedExam.Examiners, logInfoPersonalDeleteUserExam);
                }

                foreach (var deletedExamTerm in deletedExamsTerms)
                {
                    var logInfoPersonalDeleteExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExamTerm"], "Indekser " + deletedExamTerm.ExamTermIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExamTerm);

                    var logInfoPersonalDeleteUserExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserExamTerm"], "Indekser " + deletedExamTerm.ExamTermIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(deletedExamTerm.EnrolledUsers, logInfoPersonalDeleteUserExamTerm);
                    _context.personalLogRepository.AddPersonalUsersLogs(deletedExamTerm.Examiners, logInfoPersonalDeleteUserExamTerm);
                }

                foreach (var deletedExamResult in deletedExamsResults)
                {
                    var logInfoPersonalDeleteExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExamResult"], "Indekser " + deletedExamResult.ExamResultIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExamResult);

                    var logInfoPersonalDeleteUserExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserExamResult"], "Indekser " + deletedExamResult.ExamResultIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(deletedExamResult.User, logInfoPersonalDeleteUserExamTerm);
                }

                foreach (var deletedGivenCertificate in deletedGivenCertificates)
                {
                    var logInfoPersonalDeleteGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGivenCertificate);

                    var deletedGivenCertificateOwner = _context.userRepository.GetUserByGivenCertificateId(deletedGivenCertificate.GivenCertificateIdentificator);

                    var logInfoPersonalDeleteUserGivenCertificate = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserGivenCertificate"], "Indekser " + deletedGivenCertificate.GivenCertificateIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(deletedGivenCertificateOwner.Id, logInfoPersonalDeleteUserGivenCertificate);
                }

                #endregion

                return RedirectToAction("DisplayAllCourses", "Courses", new { message = "Usunięto wskazany kurs" });
            }

            return View("DeleteEntity", courseToDelete);
        }

        // GET: WorkerCourseDetails
        [Authorize(Roles = "Worker, Company")]
        public ActionResult WorkerCourseDetails(string courseIdentificator, string userIdentificator = null, string message = null)
        {
            ViewBag.Message = message;

            CertificationPlatformUser user = new CertificationPlatformUser();

            if (string.IsNullOrWhiteSpace(userIdentificator))
            {
                user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            }
            else
            {
                user = _context.userRepository.GetUserById(userIdentificator);
            }

            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            var exams = _context.examRepository.GetExamsById(course.Exams);
            var userExams = exams.Where(z => z.EnrolledUsers.Contains(user.Id)).ToList();

            var examsResults = _context.examResultRepository.GetExamsResultsById(userExams.SelectMany(z => z.ExamResults).ToList());
            var userExamsResults = examsResults.Where(z => z.User == user.Id).ToList();

            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates).Where(z => z.Course == courseIdentificator);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();
            List<DisplayMeetingWithUserPresenceInformation> meetingsPresenceViewModel = new List<DisplayMeetingWithUserPresenceInformation>();

            foreach (var meeting in meetings)
            {
                var meetingInstructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingWithoutCourseViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(meetingInstructors);

                DisplayMeetingWithUserPresenceInformation singlePresence = _mapper.Map<DisplayMeetingWithUserPresenceInformation>(meeting);

                if (meeting.AttendanceList.Contains(user.Id))
                {
                    singlePresence.IsUserPresent = true;
                }

                meetingsPresenceViewModel.Add(singlePresence);
                meetingsViewModel.Add(singleMeeting);
            }

            List<DisplayExamWithoutCourseViewModel> examsViewModel = new List<DisplayExamWithoutCourseViewModel>();

            foreach (var exam in exams)
            {
                DisplayExamWithoutCourseViewModel singleExam = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners));

                examsViewModel.Add(singleExam);
            }

            List<DisplayExamResultToUserViewModel> usersExamsWithResultsViewModel = new List<DisplayExamResultToUserViewModel>();

            foreach (var examResult in userExamsResults)
            {
                DisplayExamResultToUserViewModel singleExamResult = _mapper.Map<DisplayExamResultToUserViewModel>(examResult);

                var examRelatedWithExamResult = userExams.Where(z => z.ExamResults.Contains(examResult.ExamResultIdentificator)).FirstOrDefault();

                singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamWithDatesViewModel>(examRelatedWithExamResult);
                singleExamResult.MaxAmountOfPointsToEarn = examRelatedWithExamResult.MaxAmountOfPointsToEarn;

                singleExamResult.CanUserResignFromExam = false;

                if (!string.IsNullOrWhiteSpace(examResult.ExamTerm))
                {
                    var examTerm = _context.examTermRepository.GetExamTermById(examResult.ExamTerm);

                    singleExamResult.ExamTerm = _mapper.Map<DisplayCrucialDataExamTermViewModel>(examTerm);
                }

                usersExamsWithResultsViewModel.Add(singleExamResult);
            }

            List<Exam> userExamsWithoutResult = new List<Exam>();

            foreach (var exam in userExams)
            {
                bool userHasExamResult = false;

                foreach (var userExamResult in userExamsResults)
                {
                    if (exam.ExamResults.Contains(userExamResult.ExamResultIdentificator))
                    {
                        userHasExamResult = true;
                    }
                }

                if (userHasExamResult == false)
                {
                    userExamsWithoutResult.Add(exam);
                }
            }

            foreach (var exam in userExamsWithoutResult)
            {
                DisplayExamResultToUserViewModel singleExamResult = new DisplayExamResultToUserViewModel();

                singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamWithDatesViewModel>(exam);
                singleExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;

                singleExamResult.CanUserResignFromExam = true;

                if (DateTime.Now > exam.DateOfStart)
                {
                    singleExamResult.CanUserResignFromExam = false;
                }

                if (exam.ExamDividedToTerms)
                {
                    var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                    var userExamTerm = examTerms.Where(z => z.EnrolledUsers.Contains(user.Id)).FirstOrDefault();

                    singleExamResult.ExamTerm = _mapper.Map<DisplayCrucialDataExamTermViewModel>(userExamTerm);

                    if (DateTime.Now > userExamTerm.DateOfStart)
                    {
                        singleExamResult.CanUserResignFromExam = false;
                    }
                }

                usersExamsWithResultsViewModel.Add(singleExamResult);
            }

            var examsWithoutPeriodsIndexers = exams.GroupBy(z => z.ExamIndexer).Select(z => z.OrderBy(s => s.OrdinalNumber).FirstOrDefault()).Select(z => z.ExamIndexer).ToList();

            var passedExamsIndexers = usersExamsWithResultsViewModel.Where(z => z.ExamPassed == true && z.ExamResultIdentificator != null).Select(z => z.Exam.ExamIndexer).Distinct().ToList();

            //var failedExamsIndexers = usersExamsWithResultsViewModel.Where(z => z.ExamPassed == false && z.ExamResultIdentificator != null).Select(z => z.Exam.ExamIndexer).Distinct().ToList();

            var userExamsIndexersNotStartedOrNotMarkedYet = usersExamsWithResultsViewModel.Where(z => z.ExamPassed == false && z.ExamResultIdentificator == null).Select(z => z.Exam.ExamIndexer).ToList();

            //var allExamsWhichAreNotPassed = examsWithoutPeriods.Where(z => !z.EnrolledUsers.Contains(user.Id)).ToList();

            List<DisplayCrucialDataExamViewModel> notPassedExamsViewModel = new List<DisplayCrucialDataExamViewModel>();

            foreach (var examIndexer in examsWithoutPeriodsIndexers)
            {
                if (!passedExamsIndexers.Contains(examIndexer))
                {
                    var exam = _context.examRepository.GetExamByIndexer(examIndexer);
                    DisplayCrucialDataExamViewModel singleExam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);

                    notPassedExamsViewModel.Add(singleExam);
                }
            }

            //List<DisplayCrucialDataExamViewModel> userExamsWhichAreNotCompletedYet = new List<DisplayCrucialDataExamViewModel>();

            //foreach (var examIndexer in userExamsIndexersNotStartedOrNotMarkedYet)
            //{
            //    var exam = _context.examRepository.GetExamByIndexer(examIndexer);
            //    DisplayCrucialDataExamViewModel singleExam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);

            //    userExamsWhichAreNotCompletedYet.Add(singleExam);
            //}

            List<DisplayGivenCertificateToUserViewModel> givenCertificatesViewModel = new List<DisplayGivenCertificateToUserViewModel>();

            foreach (var givenCertificate in givenCertificates)
            {
                DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);

                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                singleGivenCertificate.Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                singleGivenCertificate.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                givenCertificatesViewModel.Add(singleGivenCertificate);
            }

            WorkerCourseDetailsViewModel courseDetails = new WorkerCourseDetailsViewModel();
            courseDetails.Course = _mapper.Map<DisplayCourseViewModel>(course);
            courseDetails.Course.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.MeetingsPresence = meetingsPresenceViewModel;

            courseDetails.Exams = examsViewModel;
            courseDetails.UserExamWithExamResults = usersExamsWithResultsViewModel;

            courseDetails.UserNotPassedExams = notPassedExamsViewModel;
            courseDetails.UserLastingExamsIndexers = userExamsIndexersNotStartedOrNotMarkedYet;

            courseDetails.Course.EnrolledUsersQuantity = course.EnrolledUsers.Count;

            courseDetails.GivenCertificates = givenCertificatesViewModel;

            return View(courseDetails);
        }

        // GET: CourseOffer
        [Authorize(Roles = "Worker, Company")]
        public ActionResult CourseOffer()
        {
            var courses = _context.courseRepository.GetListOfNotStartedCourses();
            List<DisplayCourseOfferViewModel> listOfCourses = new List<DisplayCourseOfferViewModel>();

            if (courses.Count != 0)
            {
                foreach (var course in courses)
                {
                    DisplayCourseOfferViewModel singleCourse = _mapper.Map<DisplayCourseOfferViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(singleCourse.Branches);
                    singleCourse.VacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

                    var courseQueue = _context.courseRepository.GetCourseQueueById(course.CourseIdentificator);

                    if (courseQueue != null)
                    {
                        singleCourse.VacantSeats = singleCourse.VacantSeats - courseQueue.AwaitingUsers.Count();
                    }

                    listOfCourses.Add(singleCourse);
                }
            }

            return View(listOfCourses);
        }

        // GET: CourseOfferDetails
        [Authorize(Roles = "Worker, Company")]
        public ActionResult CourseOfferDetails(string courseIdentificator, string message)
        {
            ViewBag.message = message;

            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var exams = _context.examRepository.GetFirstPeriodsExamsById(course.Exams);

            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            var instructors = _context.userRepository.GetUsersById(meetings.SelectMany(z => z.Instructors).ToList()).ToList();
            var examiners = _context.userRepository.GetUsersById(exams.SelectMany(z => z.Examiners).ToList()).ToList();

            if (user.Courses.Contains(courseIdentificator))
            {
                return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator });
            }

            var courseQueue = _context.courseRepository.GetCourseQueueById(courseIdentificator);

            if (courseQueue == null)
            {
                courseQueue = _context.courseRepository.CreateCourseQueue(courseIdentificator);
            }

            bool userInQueue = false;

            CourseOfferDetailsViewModel courseOfferDetails = new CourseOfferDetailsViewModel();

            courseOfferDetails.Course = _mapper.Map<DisplayCourseOfferViewModel>(course);
            courseOfferDetails.Course.Branches = _context.branchRepository.GetBranchesById(course.Branches);
            courseOfferDetails.Price = course.Price;
            courseOfferDetails.Course.VacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

            if (courseQueue != null)
            {
                if (courseQueue.AwaitingUsers.Select(z => z.UserIdentificator).Contains(user.Id))
                {
                    userInQueue = true;
                }

                courseOfferDetails.Course.VacantSeats = courseOfferDetails.Course.VacantSeats - courseQueue.AwaitingUsers.Count();
            }

            courseOfferDetails.UserInQueue = userInQueue;
            courseOfferDetails.Exams = _mapper.Map<List<DisplayCrucialDataExamViewModel>>(exams);

            courseOfferDetails.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors);
            courseOfferDetails.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(examiners);

            if (this.User.IsInRole("Company"))
            {
                var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());

                var companyCourseOfferDetails = _mapper.Map<CompanyCourseOfferDetailsViewModel>(courseOfferDetails);

                var courseQueueAwaitingUsers = courseQueue.AwaitingUsers.Select(z=> z.UserIdentificator).ToList();
                var companyWorkersInCourseQueue = companyWorkers.Where(z => courseQueueAwaitingUsers.Contains(z.Id)).ToList();

                companyCourseOfferDetails.CompanyWorkersInQueue = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkersInCourseQueue);

                return View("CompanyCourseOfferDetails", companyCourseOfferDetails);
            }

            return View(courseOfferDetails);
        }

        // POST: CourseOfferDetails
        [Authorize(Roles = "Worker")]
        [HttpPost]
        public async Task<ActionResult> CourseOfferDetails(CourseOfferDetailsViewModel courseOfferDetails)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var generatedCode = _keyGenerator.GenerateUserTokenForAssignToCourseQueuePurpouse(user);

            var url = Url.AssingUserToCourseVerify(user.Id, courseOfferDetails.Course.CourseIdentificator, generatedCode, Request.Scheme);

            var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "selfAssignToCourse", url, "Kurs", courseOfferDetails.Course.CourseIndexer);
            await _emailSender.SendEmailAsync(emailMessage);

            return RedirectToAction("UniversalConfirmationPanel", "Account", new { returnUrl = Url.Action("CourseOfferDetails", "Courses", new { courseIdentificator = courseOfferDetails.Course.CourseIdentificator }), messageNumber = 7 });
        }

        // GET: VerifyUserAssignToQueue
        [Authorize(Roles = "Worker")]
        public ActionResult VerifyUserAssignToQueue(string userIdentificator, string courseIdentificator, string code)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            if (user != null)
            {
                if (_keyGenerator.ValidateUserTokenForAssignToCourseQueuePurpouse(user, code))
                {
                    var courseQueue = _context.courseRepository.GetCourseQueueById(courseIdentificator);

                    if (courseQueue == null)
                    {
                        courseQueue = _context.courseRepository.CreateCourseQueue(courseIdentificator);

                        var logInfoAddCourseQueue = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addCourseQueue"]);
                        _logger.AddCourseQueueLog(courseQueue, logInfoAddCourseQueue);
                    }

                    var course = _context.courseRepository.GetCourseById(courseQueue.CourseIdentificator);

                    #region EntityLogs

                    var logInfoAddUserToCourseQueue = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["assignUserToCourseQueue"]);
                    _logger.AddCourseQueueLog(courseQueue, logInfoAddUserToCourseQueue);

                    courseQueue = _context.courseRepository.AddAwaitingUserToCourseQueue(courseIdentificator, userIdentificator, logInfoAddUserToCourseQueue);
                    #endregion

                    #region PersonalUserLogs

                    var logInfoPersonalAddUserToCourseQueue = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToCourseQueue"], "Indekser kursu: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToCourseQueue);

                    var logInfoPersonalAssignUserToCourseQueue = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToCourseQueue"], "Indekser kursu: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalAssignUserToCourseQueue);

                    #endregion

                    return RedirectToAction("CourseOfferDetails", "Courses", new { courseIdentificator, message = "Przyjęto Twoje zgłoszenie o zapisanie na kurs. Po uiszczeniu opłaty otrzymasz do niego dostęp." });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: MoveUserFromCourseQueueToCourse
        [Authorize(Roles = "Admin")]
        public ActionResult MoveUserFromCourseQueueToCourse(string userIdentificator, string courseIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);
            var courseQueue = _context.courseRepository.GetCourseQueueById(courseIdentificator);

            if (courseQueue != null && user != null)
            {
                if (!courseQueue.AwaitingUsers.Select(z => z.UserIdentificator).Contains(userIdentificator))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                var course = _context.courseRepository.GetCourseById(courseIdentificator);

                if (!course.EnrolledUsers.Contains(userIdentificator) && (course.EnrolledUsersLimit - course.EnrolledUsers.Count) > 0)
                {
                    course = _context.courseRepository.AddEnrolledUserToCourse(courseIdentificator, userIdentificator);
                    user = _context.userRepository.AddUserToCourse(courseIdentificator, userIdentificator);

                    #region EntityLogs

                    var logInfoUpdateUser = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToCourse"]);
                    _logger.AddUserLog(user, logInfoUpdateUser);

                    var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addUserToCourse"]);
                    _logger.AddCourseLog(course, logInfoUpdateCourse);

                    #endregion

                    #region PersonalUserLogs

                    var logInfoPersonalAddUserToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToCourse"], "Indekser: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToCourse);

                    var logInfoPersonalAssignUserToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToCourse"], "Indekser: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalAssignUserToCourse);

                    #endregion

                    return RedirectToAction("DeleteUserFromCourseQueue", "Courses", new { userIdentificator, courseIdentificator, userAssignedToCourse = true });
                }

                return RedirectToAction("BlankMenu", "Certificates");

            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteUserFromCourseQueue
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUserFromCourseQueue(string userIdentificator, string courseIdentificator, bool userAssignedToCourse)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);
            var courseQueue = _context.courseRepository.GetCourseQueueById(courseIdentificator);

            if (courseQueue != null && user != null)
            {
                if (!courseQueue.AwaitingUsers.Select(z => z.UserIdentificator).Contains(userIdentificator))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                var userLogDataOfAssignToCourseQueueAction = courseQueue.AwaitingUsers.Where(z => z.UserIdentificator == userIdentificator).Select(z => z.LogData).FirstOrDefault();
                courseQueue = _context.courseRepository.RemoveAwaitingUserFromCourseQueue(courseIdentificator, userIdentificator);

                var logInfoRemoveUserFromCourseQueue = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromCourseQueue"]);
                _logger.AddCourseQueueLog(courseQueue, logInfoRemoveUserFromCourseQueue);

                if (courseQueue.AwaitingUsers.Count() == 0)
                {
                    _context.courseRepository.DeleteCourseQueue(courseIdentificator);

                    var logInfoDeleteCourseQueue = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteCourseQueue"]);
                    _logger.AddCourseQueueLog(courseQueue, logInfoDeleteCourseQueue);
                }

                if (userAssignedToCourse)
                {
                    return RedirectToAction("AdminNotificationManager", "Courses", new { message = "Zgłoszenie pracownika zostało przyjęte." });
                }
                else
                {
                    var course = _context.courseRepository.GetCourseById(courseIdentificator);

                    #region PersonalUserLogs

                    CourseQueueWithSingleUser rejectedUser = new CourseQueueWithSingleUser();
                    rejectedUser.CourseIdentificator = courseIdentificator;
                    rejectedUser.UserIdentificator = userIdentificator;
                    rejectedUser.LogDataOfAssignToCourseQueue = userLogDataOfAssignToCourseQueueAction;

                    var rejectedUserLog = new RejectedUserFromCourseQueueLog
                    {
                        RejectedUserFromCourseQueueLogIdentificator = _keyGenerator.GenerateNewId(),
                        AlteredEntity = rejectedUser,
                        LogData = logInfoRemoveUserFromCourseQueue
                    };

                    _context.personalLogRepository.AddRejectedUserLog(rejectedUserLog);

                    var logInfoPersonalAddRejectedUser = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addRejectedUser"], "Indekser: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddRejectedUser);

                    var logInfoPersonalRejectUserFromCourseQueue = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["rejectUserFromCourseQueue"], "Indekser: " + course.CourseIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalRejectUserFromCourseQueue);

                    #endregion

                    return RedirectToAction("AdminNotificationManager", "Courses", new { message = "Zgłoszenie pracownika zostało odrzucone." });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: CompanyCourseDetails
        [Authorize(Roles = "Company")]
        public ActionResult CompanyCourseDetails(string courseIdentificator)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(user.CompanyRoleManager.FirstOrDefault());

            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

            foreach (var meeting in meetings)
            {
                var meetingInstructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingWithoutCourseViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(meetingInstructors);

                meetingsViewModel.Add(singleMeeting);
            }

            var users = _context.userRepository.GetUsersById(course.EnrolledUsers);
            var companyWorkersEnrolledInCourse = companyWorkers.Where(z => z.Courses.Contains(courseIdentificator)).ToList();

            List<DisplayCrucialDataUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkersEnrolledInCourse);

            var instructorsIdentificators = new List<string>();
            meetings.ToList().ForEach(z => z.Instructors.ToList().ForEach(s => instructorsIdentificators.Add(s)));

            var instructors = _context.userRepository.GetUsersById(instructorsIdentificators.Distinct().ToList());
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(instructors);

            var exams = _context.examRepository.GetExamsById(course.Exams);
            List<DisplayExamWithoutCourseViewModel> examsViewModel = new List<DisplayExamWithoutCourseViewModel>();

            List<string> listOfExaminersIdentificators = new List<string>();

            // todo: speed up
            foreach (var exam in exams)
            {
                DisplayExamWithoutCourseViewModel singleExam = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners));

                var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                examTerms.ToList().ForEach(z => listOfExaminersIdentificators.AddRange(z.Examiners));
                listOfExaminersIdentificators.AddRange(exam.Examiners);

                examsViewModel.Add(singleExam);
            }

            listOfExaminersIdentificators.Distinct();

            var examiners = _context.userRepository.GetUsersById(listOfExaminersIdentificators);
            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(examiners);

            CourseDetailsViewModel courseDetails = new CourseDetailsViewModel();
            courseDetails.Course = _mapper.Map<DisplayCourseWithPriceViewModel>(course);
            courseDetails.Course.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.Exams = examsViewModel;

            courseDetails.Course.EnrolledUsersQuantity = course.EnrolledUsers.Count;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;
            courseDetails.Examiners = examinersViewModel;

            return View(courseDetails);
        }

        // GET: DisplayWorkerCourses
        [Authorize(Roles = "Company")]
        public ActionResult DisplayWorkerCourses()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(user.CompanyRoleManager.FirstOrDefault());

            var courses = _context.courseRepository.GetCoursesById(companyWorkers.SelectMany(z => z.Courses).ToList()).ToList();

            List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

            if (courses.Count != 0)
            {
                listOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(courses);
                listOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                listOfCourses.ForEach(z => z.EnrolledUsersQuantity = courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(listOfCourses);
        }


        // GET: DisplayCompanyWorkersCourseSummary
        [Authorize(Roles = "Company")]
        public ActionResult DisplayCompanyWorkersCourseSummary(string courseIdentificator)
        {
            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());

            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

            var exams = _context.examRepository.GetExamsById(course.Exams);

            var companyWorkersEnrolledInCourse = companyWorkers.Where(z => z.Courses.Contains(courseIdentificator)).ToList();
            var companyWorkersEnrolledInCourseIdentificators = companyWorkersEnrolledInCourse.Select(z => z.Id).ToList();

            var companyWorkersGivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(companyWorkersEnrolledInCourse.SelectMany(z=> z.GivenCertificates).ToList()).Where(z => z.Course == courseIdentificator);
            var companyWorkersExamsResults = _context.examResultRepository.GetExamsResultsById(exams.SelectMany(z => z.ExamResults).ToList()).Where(z=> companyWorkersEnrolledInCourseIdentificators.Contains(z.User)).ToList();

            List<DisplayUserWithCourseResultsViewModel> listOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            if (companyWorkersEnrolledInCourse.Count != 0)
            {
                listOfUsers = GetCourseListOfUsersWithAllStatistics(companyWorkersEnrolledInCourse, meetings, exams).ToList();
            }

            List<DisplayGivenCertificateWithoutCourseViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateWithoutCourseViewModel>();

            foreach (var givenCertificate in companyWorkersGivenCertificates)
            {
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var user = companyWorkers.Where(z => z.GivenCertificates.Contains(givenCertificate.GivenCertificateIdentificator)).FirstOrDefault();

                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);

                DisplayGivenCertificateWithoutCourseViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateWithoutCourseViewModel>(givenCertificate);
                singleGivenCertificate.Certificate = certificateViewModel;
                singleGivenCertificate.User = userViewModel;

                listOfGivenCertificates.Add(singleGivenCertificate);
            }

            var examsPeriods = _context.examRepository.GetFirstPeriodsExamsById(exams.Select(z=> z.ExamIdentificator).ToList());
            var listOfUsersWithExamPeriodsStatus = new List<DisplayUserWithCourseExamPeriodsResultsViewModel>();

            foreach (var companyWorker in companyWorkersEnrolledInCourse)
            {
                var singleCompanyWorkerWithExamsPeriodsStatus = _mapper.Map<DisplayUserWithCourseExamPeriodsResultsViewModel>(companyWorker);

                var singleCompanyWorkerResults = companyWorkersExamsResults.Where(z => z.User == companyWorker.Id).ToList();

                foreach (var examPeriod in examsPeriods)
                {
                    var examsFromExamPeriod = exams.Where(z => z.ExamIndexer == examPeriod.ExamIndexer).ToList();

                    var companyWorkersExamsFromExamPeriod = examsFromExamPeriod.Where(z => z.EnrolledUsers.Contains(companyWorker.Id)).ToList();
                    var companyWorkerResultsFromExamPeriod = singleCompanyWorkerResults.Where(z => examsFromExamPeriod.SelectMany(x=> x.ExamResults).ToList().Contains(z.ExamResultIdentificator)).ToList();

                    if (companyWorkerResultsFromExamPeriod.Count() != companyWorkersExamsFromExamPeriod.Count())
                    {
                        continue;
                    }
                    else if (companyWorkerResultsFromExamPeriod.Where(z=> z.ExamPassed == true).Count() == 1)
                    {
                        singleCompanyWorkerWithExamsPeriodsStatus.ExamsIndexersPassed.Add(examPeriod.ExamIndexer);
                    }
                    else 
                    {
                        singleCompanyWorkerWithExamsPeriodsStatus.LastingExamsIndexers.Add(examPeriod.ExamIndexer);
                    }
                }

                listOfUsersWithExamPeriodsStatus.Add(singleCompanyWorkerWithExamsPeriodsStatus);
            }

            DisplayCompanyCourseSummaryViewModel courseSummaryViewModel = _mapper.Map<DisplayCompanyCourseSummaryViewModel>(course);
            courseSummaryViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            courseSummaryViewModel.AllCourseCompanyWorkers = listOfUsers;
            courseSummaryViewModel.CompanyWorkersWithExamPeriodStatus = listOfUsersWithExamPeriodsStatus;

            courseSummaryViewModel.AllCourseExams = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(exams.OrderBy(z => z.ExamIndexer));
            courseSummaryViewModel.LastExamsPeriods = _mapper.Map<List<DisplayExamIndexerWithOrdinalNumberViewModel>>(examsPeriods);

            courseSummaryViewModel.CourseLength = courseSummaryViewModel.DateOfEnd.Subtract(courseSummaryViewModel.DateOfStart).Days;
            courseSummaryViewModel.EnrolledUsersQuantity = course.EnrolledUsers.Count;

            courseSummaryViewModel.GivenCertificates = listOfGivenCertificates;
            courseSummaryViewModel.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(listOfUsers);

            foreach (var givenCertificate in listOfGivenCertificates)
            {
                courseSummaryViewModel.DispensedGivenCertificates.Where(z => z.UserIdentificator == givenCertificate.User.UserIdentificator).FirstOrDefault().GivenCertificateIsEarned = true;
            }

            return View(courseSummaryViewModel);
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
                    var lastUserResult = _context.examResultRepository.GetExamsResultsById(lastExamPeriod.ExamResults).ToList().Where(z => z.User == user.Id).FirstOrDefault();

                    if (lastUserResult != null)
                    {
                        var singleExamResult = _mapper.Map<DisplayExamResultWithExamIdentificator>(lastUserResult);
                        singleExamResult.MaxAmountOfPointsToEarn = lastExamPeriod.MaxAmountOfPointsToEarn;

                        singleExamResult = _mapper.Map<Exam, DisplayExamResultWithExamIdentificator>(lastExamPeriod, singleExamResult);

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

                            singleExamResult = _mapper.Map<Exam, DisplayExamResultWithExamIdentificator>(exam, singleExamResult);

                            singleUser.ExamsResults.Add(singleExamResult);
                        }
                    }
                }

                listOfUsers.Add(singleUser);
            }

            return listOfUsers;
        }
        #endregion
    }
}


