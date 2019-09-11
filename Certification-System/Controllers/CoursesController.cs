using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;
using System;
using Certification_System.ServicesInterfaces.Models;

namespace Certification_System.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public CoursesController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: ConfirmationOfActionOnCourse
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCourse(string courseIdentificator, string TypeOfAction)
        {
            if (courseIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Course = _context.courseRepository.GetCourseById(courseIdentificator);

                DisplayCourseWithMeetingsViewModel modifiedCourse = new DisplayCourseWithMeetingsViewModel();
                modifiedCourse.Course = _mapper.Map<DisplayCourseViewModel>(Course);

                if (Course.Meetings.Count() != 0)
                {
                    var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);
                    modifiedCourse.Meetings = _mapper.Map<List<DisplayMeetingWithoutCourseViewModel>>(Meetings);

                    foreach (var meeting in modifiedCourse.Meetings)
                    {
                        var Instructors = _context.userRepository.GetUsersById(Meetings.Where(z => z.MeetingIdentificator == meeting.MeetingIdentificator).Select(z => z.Instructors).FirstOrDefault());

                        meeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Instructors);
                    }
                }

                var BranchNames = _context.branchRepository.GetBranchesById(Course.Branches);
                modifiedCourse.Course.Branches = BranchNames;

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

                if (course.DateOfEnd != null)
                {
                    course.CourseLength = course.DateOfEnd.Subtract(course.DateOfStart).Days;
                }

                _context.courseRepository.AddCourse(course);

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

                if (course.DateOfEnd != null)
                {
                    course.CourseLength = course.DateOfEnd.Subtract(course.DateOfStart).Days;
                }

                List<Meeting> meetings = new List<Meeting>();

                if (course.Meetings.Count() != 0)
                {
                    foreach (var newMeeting in course.Meetings)
                    {
                        Meeting meeting = _mapper.Map<Meeting>(newMeeting);
                        meeting.MeetingIdentificator = _keyGenerator.GenerateNewId();

                        meetings.Add(meeting);
                    }

                    _context.meetingRepository.AddMeetings(meetings);
                }

                var MeetingsIdentificators = meetings.Select(z => z.MeetingIdentificator);

                course.Meetings.ToList().AddRange(MeetingsIdentificators);

                _context.courseRepository.AddCourse(course);

                return RedirectToAction("ConfirmationOfActionOnCourse", new { courseIdentificator = course.CourseIdentificator, TypeOfAction = "Add" });
            }

            newCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            newCourse.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();

            return View(newCourse);
        }

        // GET: DisplayAllCourses
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllCourses()
        {
            var Courses = _context.courseRepository.GetListOfCourses();
            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(Courses);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(ListOfCourses);
        }

        // GET: CourseDetails
        [Authorize(Roles = "Admin, Instructor, Examiner")]
        public ActionResult CourseDetails(string courseIdentificator, string message)
        {
            ViewBag.Message = message;

            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

            // przemyśleć nad zmniejszeniem ViewModelu -> niepotrzebne pola
            foreach (var meeting in Meetings)
            {
                var meetingInstructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingWithoutCourseViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(meetingInstructors);

                meetingsViewModel.Add(singleMeeting);
            }

            var Users = _context.userRepository.GetUsersById(Course.EnrolledUsers);

            List<DisplayCrucialDataUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Users);
            //usersViewModel.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).Select(s => s.CompanyName).ToList());
            //usersViewModel.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).Select(s => s.CompanyName).ToList());

            var InstructorsIdentificators = new List<string>();
            Meetings.ToList().ForEach(z => z.Instructors.ToList().ForEach(s => InstructorsIdentificators.Add(s)));

            var Instructors = _context.userRepository.GetUsersById(InstructorsIdentificators.Distinct().ToList());
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Instructors);

            var Exams = _context.examRepository.GetExamsById(Course.Exams);
            List<DisplayExamWithoutCourseViewModel> examsViewModel = new List<DisplayExamWithoutCourseViewModel>();

            foreach (var exam in Exams)
            {
                DisplayExamWithoutCourseViewModel singleExam = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners));

                examsViewModel.Add(singleExam);
            }

            List<string> ListOfExaminatorsIdentificators = new List<string>();

            foreach (var exam in Exams)
            {
                var ExamTermsOfExam = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                ExamTermsOfExam.ToList().ForEach(z => ListOfExaminatorsIdentificators.AddRange(z.Examiners));

                ListOfExaminatorsIdentificators.AddRange(exam.Examiners);
            }
            ListOfExaminatorsIdentificators.Distinct();

            var Examiners = _context.userRepository.GetUsersById(ListOfExaminatorsIdentificators);
            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Examiners);

            CourseDetailsViewModel courseDetails = new CourseDetailsViewModel();
            courseDetails.Course = _mapper.Map<DisplayCourseViewModel>(Course);
            courseDetails.Course.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.Exams = examsViewModel;

            courseDetails.Course.EnrolledUsersQuantity = Course.EnrolledUsers.Count;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;
            courseDetails.Examiners = examinersViewModel;

            courseDetails.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(usersViewModel);

            if (Course.CourseEnded)
            {
                foreach (var user in Users)
                {
                    var UsersGivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

                    if (UsersGivenCertificates.Where(z => z.Course == Course.CourseIdentificator).Count() != 0)
                    {
                        courseDetails.DispensedGivenCertificates.Where(z => z.UserIdentificator == user.Id).Select(z => z.GivenCertificateIsEarned = true);
                    }
                }
            }

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
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);

            if (Course.Meetings.Count() != 0)
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
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);

            EditCourseViewModel courseToUpdate = _mapper.Map<EditCourseViewModel>(Course);
            courseToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            return View(courseToUpdate);
        }

        // POST: EditCourse
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCourse(EditCourseViewModel editedCourse)
        {
            var OriginCourse = _context.courseRepository.GetCourseById(editedCourse.CourseIdentificator);

            if (ModelState.IsValid)
            {
                OriginCourse = _mapper.Map<EditCourseViewModel, Course>(editedCourse, OriginCourse);

                if (OriginCourse.DateOfEnd != null)
                {
                    OriginCourse.CourseLength = OriginCourse.DateOfEnd.Subtract(OriginCourse.DateOfStart).Days;
                }

                _context.courseRepository.UpdateCourse(OriginCourse);

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
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            EditCourseWithMeetingsViewModel courseToUpdate = _mapper.Map<EditCourseWithMeetingsViewModel>(Course);
            courseToUpdate.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            courseToUpdate.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();
            courseToUpdate.Meetings = _mapper.Map<List<EditMeetingViewModel>>(Meetings);

            return View(courseToUpdate);
        }

        // POST: EditCourseWithMeetings
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCourseWithMeetings(EditCourseWithMeetingsViewModel editedCourse)
        {
            var OriginCourse = _context.courseRepository.GetCourseById(editedCourse.CourseIdentificator);
            var OriginMeetings = _context.meetingRepository.GetMeetingsById(editedCourse.Meetings.Select(z => z.MeetingIdentificator).ToList());

            if (ModelState.IsValid)
            {
                OriginMeetings = _mapper.Map<List<EditMeetingViewModel>, List<Meeting>>(editedCourse.Meetings.ToList(), OriginMeetings.ToList());

                _context.meetingRepository.UpdateMeetings(OriginMeetings);

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
            ICollection<string> ChosenUsers;
            string ChosenCourse = "";

            if (userIdentificators.Count() != 0)
                ChosenUsers = _context.userRepository.GetUsersById(userIdentificators).Select(z => z.Id).ToList();
            else
                ChosenUsers = new List<string>();

            if (courseIdentificator != null)
                ChosenCourse = _context.courseRepository.GetCourseById(courseIdentificator).CourseIdentificator;

            var AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            var AvailableCourses = _context.courseRepository.GetActiveCoursesWithVacantSeatsAsSelectList().ToList();

            AssignUserToCourseViewModel usersToAssignToCourse = new AssignUserToCourseViewModel
            {
                AvailableCourses = AvailableCourses,
                SelectedCourse = ChosenCourse,

                AvailableUsers = AvailableUsers,
                SelectedUsers = ChosenUsers
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
                var UsersAlreadyInChosenCourse = _context.userRepository.GetUsersById(usersAssignedToCourse.SelectedUsers.ToList()).Where(z => z.Courses.Contains(usersAssignedToCourse.SelectedCourse)).ToList();

                if (UsersAlreadyInChosenCourse.Count() == 0)
                {
                    var Course = _context.courseRepository.GetCourseById(usersAssignedToCourse.SelectedCourse);
                    var VacantSeats = Course.EnrolledUsersLimit - Course.EnrolledUsers.Count();

                    if (VacantSeats < usersAssignedToCourse.SelectedUsers.Count())
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranych użytkowników");
                        ModelState.AddModelError("", $"Do wybranego kursu maksymalnie możesz zapisać jeszcze: {VacantSeats} użytkowników");
                    }
                    else
                    {
                        _context.userRepository.AddUsersToCourse(usersAssignedToCourse.SelectedCourse, usersAssignedToCourse.SelectedUsers);
                        _context.courseRepository.AddEnrolledUsersToCourse(usersAssignedToCourse.SelectedCourse, usersAssignedToCourse.SelectedUsers);

                        return RedirectToAction("CourseDetails", new { courseIdentificator = usersAssignedToCourse.SelectedCourse, message = "Zapisano nowych użytkowników na kurs" });
                    }
                }
                else
                {
                    foreach (var user in UsersAlreadyInChosenCourse)
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
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            var Exams = _context.examRepository.GetExamsById(Course.Exams);

            if (Course.CourseEnded != true)
            {
                var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);

                List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

                if (EnrolledUsersList.Count != 0)
                {
                    ListOfUsers = GetCourseListOfUsersWithStatistics(EnrolledUsersList, Meetings, Exams).ToList();
                }

                DispenseGivenCertificatesViewModel courseToEndViewModel = _mapper.Map<DispenseGivenCertificatesViewModel>(Course);
                courseToEndViewModel.AllCourseParticipants = ListOfUsers;
                courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

                courseToEndViewModel.CourseLength = courseToEndViewModel.DateOfEnd.Subtract(courseToEndViewModel.DateOfStart).Days;
                courseToEndViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);
                courseToEndViewModel.EnrolledUsersQuantity = Course.EnrolledUsers.Count;

                courseToEndViewModel.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(ListOfUsers);

                return View(courseToEndViewModel);
            }

            return RedirectToAction("CourseDetails", new { courseIdentificator = courseIdentificator });
        }

        // POST: EndCourseAndDispenseGivenCertificates
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EndCourseAndDispenseGivenCertificates(DispenseGivenCertificatesViewModel courseToEndViewModel)
        {
            var Course = _context.courseRepository.GetCourseById(courseToEndViewModel.CourseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            if (ModelState.IsValid)
            {
                Course.CourseEnded = true;

                _context.courseRepository.UpdateCourse(Course);

                for (int i = 0; i < courseToEndViewModel.DispensedGivenCertificates.Count(); i++)
                {
                    if (courseToEndViewModel.DispensedGivenCertificates[i].GivenCertificateIsEarned == true)
                    {
                        GivenCertificate singleGivenCertificate = new GivenCertificate
                        {
                            GivenCertificateIdentificator = _keyGenerator.GenerateNewId(),
                            //GivenCertificateIndexer = from generator
                            GivenCertificateIndexer = "AAAAAAAAAA",

                            ReceiptDate = courseToEndViewModel.ReceiptDate,
                            ExpirationDate = courseToEndViewModel.ExpirationDate,

                            Depreciated = false,
                            Course = courseToEndViewModel.CourseIdentificator,
                            Certificate = courseToEndViewModel.SelectedCertificate
                        };

                        _context.givenCertificateRepository.AddGivenCertificate(singleGivenCertificate);

                        var User = _context.userRepository.GetUserById(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator);
                        User.GivenCertificates.Add(singleGivenCertificate.GivenCertificateIdentificator);

                        _context.userRepository.UpdateUser(User);
                    }
                }

                return RedirectToAction("CourseDetails", new { courseIdentificator = courseToEndViewModel.CourseIdentificator, message = "Zaknięto kurs i rozdano certyfikaty." });
            }

            var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);
            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            var Exams = _context.examRepository.GetExamsById(Course.Exams);

            List<string> AllExamsResultsIdentificators = new List<string>();
            Exams.ToList().ForEach(z => AllExamsResultsIdentificators.AddRange(z.ExamResults));
            var AllExamsResults = _context.examResultRepository.GetExamsResultsById(AllExamsResultsIdentificators);

            if (EnrolledUsersList.Count != 0)
            {
                ListOfUsers = GetCourseListOfUsersWithStatistics(EnrolledUsersList, Meetings, Exams).ToList();
            }

            courseToEndViewModel.AllCourseParticipants = ListOfUsers;
            courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

            return View(courseToEndViewModel);
        }

        // GET: DisplayCourseSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayCourseSummary(string courseIdentificator)
        {
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            var Exams = _context.examRepository.GetExamsById(Course.Exams);

            var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);

            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            if (EnrolledUsersList.Count != 0)
            {
                ListOfUsers = GetCourseListOfUsersWithAllStatistics(EnrolledUsersList, Meetings, Exams).ToList();
            }

            DisplayCourseSummaryViewModel courseSummaryViewModel = _mapper.Map<DisplayCourseSummaryViewModel>(Course);
            courseSummaryViewModel.AllCourseParticipants = ListOfUsers;
            courseSummaryViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            courseSummaryViewModel.ExamsQuantity = Course.Exams.Count();
            courseSummaryViewModel.CourseLength = courseSummaryViewModel.DateOfEnd.Subtract(courseSummaryViewModel.DateOfStart).Days;
            courseSummaryViewModel.EnrolledUsersQuantity = Course.EnrolledUsers.Count;

            return View(courseSummaryViewModel);
        }

        // GET: DeleteUserFromCourse
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromCourse(string courseIdentificator)
        {
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);

            if (Course.CourseEnded != true)
            {
                var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (EnrolledUsersList.Count != 0)
                {
                    ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(EnrolledUsersList);
                }

                DeleteUsersFromCourseViewModel deleteUsersFromCourseViewModel = _mapper.Map<DeleteUsersFromCourseViewModel>(Course);
                deleteUsersFromCourseViewModel.AllCourseParticipants = ListOfUsers;

                deleteUsersFromCourseViewModel.CourseLength = deleteUsersFromCourseViewModel.DateOfEnd.Subtract(deleteUsersFromCourseViewModel.DateOfStart).Days;
                deleteUsersFromCourseViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);
                deleteUsersFromCourseViewModel.EnrolledUsersQuantity = Course.EnrolledUsers.Count;

                deleteUsersFromCourseViewModel.UsersToDeleteFromCourse = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(ListOfUsers);

                return View(deleteUsersFromCourseViewModel);
            }

            return RedirectToAction("CourseDetails", new { courseIdentificator = courseIdentificator });
        }

        // POST: DeleteUserFromCourse
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromCourse(DeleteUsersFromCourseViewModel deleteUsersFromCourseViewModel)
        {
            if (ModelState.IsValid)
            {
                var UsersToDeleteFromCourseIdentificators = deleteUsersFromCourseViewModel.UsersToDeleteFromCourse.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                _context.courseRepository.DeleteUsersFromCourse(deleteUsersFromCourseViewModel.CourseIdentificator, UsersToDeleteFromCourseIdentificators);
                _context.userRepository.DeleteCourseFromUsersCollection(deleteUsersFromCourseViewModel.CourseIdentificator, UsersToDeleteFromCourseIdentificators);

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
            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Courses = _context.courseRepository.GetCoursesById(User.Courses);
            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(Courses);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(ListOfCourses);
        }

        // GET: InstructorCourses
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorCourses()
        {
            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Meetings = _context.meetingRepository.GetMeetingsByInstructorId(User.Id);
            var Courses = _context.courseRepository.GetCoursesByMeetingsId(Meetings.Select(z => z.MeetingIdentificator).ToList());

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(Courses);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(ListOfCourses);
        }

        // GET: ExaminerCourses
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerCourses()
        {
            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Exams = _context.examRepository.GetExamsByExaminerId(User.Id);
            var Courses = _context.courseRepository.GetCoursesByExamsId(Exams.Select(z => z.ExamIdentificator).ToList()).ToList();

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                ListOfCourses = _mapper.Map<List<DisplayCourseViewModel>>(Courses);
                ListOfCourses.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCourses.ForEach(z => z.EnrolledUsersQuantity = Courses.Where(s => s.CourseIdentificator == z.CourseIdentificator).FirstOrDefault().EnrolledUsers.Count);
            }

            return View(ListOfCourses);
        }

        // GET: InstructorExaminerCourses
        [Authorize(Roles = "Instructor")]
        [Authorize(Roles = "Examiner")]
        public ActionResult InstructorExaminerCourses()
        {
            ViewBag.AvailableRoleFilters = _context.userRepository.GetAvailableCourseRoleFiltersAsSelectList();

            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Exams = _context.examRepository.GetExamsByExaminerId(User.Id);
            var CoursesAsExaminer = _context.courseRepository.GetCoursesByExamsId(Exams.Select(z => z.ExamIdentificator).ToList()).ToList();

            var Meetings = _context.meetingRepository.GetMeetingsByInstructorId(User.Id);
            var CoursesAsInstructor = _context.courseRepository.GetCoursesByMeetingsId(Meetings.Select(z => z.MeetingIdentificator).ToList());

            var BothRolesCourses = CoursesAsExaminer.Intersect(CoursesAsInstructor).ToList();

            foreach (var course in BothRolesCourses)
            {
                CoursesAsExaminer.Remove(course);
                CoursesAsInstructor.Remove(course);
            }

            List<DisplayCourseWithUserRoleViewModel> ListOfCoursesAsExaminer = new List<DisplayCourseWithUserRoleViewModel>();

            if (CoursesAsExaminer.Count != 0)
            {
                ListOfCoursesAsExaminer = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(CoursesAsExaminer);
                ListOfCoursesAsExaminer.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCoursesAsExaminer.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Examiner"]));
            }

            List<DisplayCourseWithUserRoleViewModel> ListOfCoursesAsInstructor = new List<DisplayCourseWithUserRoleViewModel>();

            if (CoursesAsInstructor.Count != 0)
            {
                ListOfCoursesAsInstructor = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(CoursesAsInstructor);
                ListOfCoursesAsInstructor.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));
                ListOfCoursesAsInstructor.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Instructor"]));
            }

            List<DisplayCourseWithUserRoleViewModel> ListOfCoursesAsBothRoles = new List<DisplayCourseWithUserRoleViewModel>();

            if (BothRolesCourses.Count != 0)
            {
                ListOfCoursesAsBothRoles = _mapper.Map<List<DisplayCourseWithUserRoleViewModel>>(CoursesAsInstructor);
                ListOfCoursesAsBothRoles.ForEach(z => z.Branches = _context.branchRepository.GetBranchesById(z.Branches));

                ListOfCoursesAsBothRoles.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Instructor"]));
                ListOfCoursesAsBothRoles.ForEach(z => z.Roles.Add(UserRolesDictionary.TranslationDictionary["Examiner"]));
            }

            List<DisplayCourseWithUserRoleViewModel> AllCourses = new List<DisplayCourseWithUserRoleViewModel>();

            AllCourses.AddRange(ListOfCoursesAsExaminer);
            AllCourses.AddRange(ListOfCoursesAsInstructor);
            AllCourses.AddRange(ListOfCoursesAsBothRoles);

            return View(AllCourses);
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
                coursesArray[i][1] = courses[i].CourseIndexer + " " + courses[i].Name;
            }

            return coursesArray;
        }
        #endregion

        #region HelpersMethod
        // GetCourseListOfUsersWithStatistics
        private ICollection<DisplayUserWithCourseResultsViewModel> GetCourseListOfUsersWithStatistics(ICollection<CertificationPlatformUser> enrolledUsers, ICollection<Meeting> meetings, ICollection<Exam> exams)
        {
            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

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

                var UserExams = exams.Where(z => z.EnrolledUsers.Contains(user.Id)).GroupBy(z => z.ExamIndexer).ToList();

                for (int i = 0; i < UserExams.Count; i++)
                {
                    var LastExamPeriod = UserExams[i].ToList().OrderByDescending(z => z.OrdinalNumber).FirstOrDefault();
                    var LastUserResult = _context.examResultRepository.GetExamsResultsById(LastExamPeriod.ExamResults).ToList().Where(z => z.User == user.Id).ToList();

                    if (LastUserResult != null)
                    {
                        var singleExamResult = _mapper.Map<DisplayExamResultWithExamNumber>(LastUserResult);
                        singleExamResult.MaxAmountOfPointsToEarn = LastExamPeriod.MaxAmountOfPointsToEarn;
                        singleExamResult.ExamOrdinalNumber = LastExamPeriod.OrdinalNumber;

                        singleUser.ExamsResults.Add(singleExamResult);
                    }
                }

                ListOfUsers.Add(singleUser);
            }

            return ListOfUsers;
        }

        // GetCourseListOfUsersWithAllStatistics
        private ICollection<DisplayUserWithCourseResultsViewModel> GetCourseListOfUsersWithAllStatistics(ICollection<CertificationPlatformUser> enrolledUsers, ICollection<Meeting> meetings, ICollection<Exam> exams)
        {
            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

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

                var UserExams = exams.Where(z => z.EnrolledUsers.Contains(user.Id)).GroupBy(z => z.ExamIndexer).ToList();

                for (int i = 0; i < UserExams.Count; i++)
                {
                    foreach (var exam in UserExams[i])
                    {
                        var UserResult = _context.examResultRepository.GetExamsResultsById(exam.ExamResults).ToList().Where(z => z.User == user.Id).ToList();

                        if (UserResult != null)
                        {
                            var singleExamResult = _mapper.Map<DisplayExamResultWithExamNumber>(UserResult);
                            singleExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;
                            singleExamResult.ExamOrdinalNumber = exam.OrdinalNumber;

                            singleUser.ExamsResults.Add(singleExamResult);
                        }

                        ListOfUsers.Add(singleUser);
                    }
                }
            }

            return ListOfUsers;
        }
        #endregion
    }
}


