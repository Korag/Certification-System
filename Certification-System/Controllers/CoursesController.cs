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

        // GET: AddNewCourse
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCourse()
        {
            AddCourseViewModel newCourse = new AddCourseViewModel
            {
                AvailableBranches = new List<SelectListItem>(),
                SelectedBranches = new List<string>(),
                Meetings = new List<AddMeetingViewModel>()
            };

            newCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();

            //todo GetInstructorsData and store it in some collection in AddCourseViewModel

            return View(newCourse);
        }

        // GET: ConfirmationOfActionOnCourse
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnCourse(string courseIdentificator, ICollection<string> meetingsIdentificators, string TypeOfAction)
        {
            if (courseIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Course = _context.courseRepository.GetCourseById(courseIdentificator);

                DisplayCourseWithMeetingsViewModel modifiedCourse = _mapper.Map<DisplayCourseWithMeetingsViewModel>(Course);

                if (Course.Meetings != null)
                {
                    //    var MeetingsId = _context.GetMeetingsById(Course.Meetings);
                    //    List<AddMeetingViewModel> Meetings = new List<AddMeetingViewModel>();

                    //    foreach (var meeting in MeetingsId)
                    //    {
                    //        var Instructors = _context.GetInstructorsById(meeting.Instructor);

                    //        AddMeetingViewModel meetingsInCourse = new AddMeetingViewModel
                    //        {
                    //            MeetingIndexer = meeting.MeetingIndexer,
                    //            Description = meeting.Description,
                    //            DateOfMeeting = meeting.DateOfMeeting,
                    //            Country = meeting.Country,
                    //            City = meeting.City,
                    //            PostCode = meeting.PostCode,
                    //            Address = meeting.Address,
                    //            NumberOfApartment = meeting.NumberOfApartment,

                    //            Instructors = new List<string>()
                    //        };

                    //        foreach (var instructor in Instructors)
                    //        {
                    //            string instructorIdentity = instructor.FirstName + instructor.LastName;
                    //            meetingsInCourse.Instructors.Add(instructorIdentity);
                    //        }

                    //        Meetings.Add(meetingsInCourse);
                    //    }

                    //    addedCourse.MeetingsViewModels = Meetings;
                }

                var BranchNames = _context.branchRepository.GetBranchesById(Course.Branches);
                modifiedCourse.Branches = BranchNames;

                return View(modifiedCourse);
            }

            return RedirectToAction(nameof(AddNewCourse));
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

                if (newCourse.Meetings != null)
                {
                    foreach (var meeting in newCourse.Meetings)
                    {
                        Meeting singleMeeting = _mapper.Map<Meeting>(meeting);
                        singleMeeting.MeetingIdentificator = _keyGenerator.GenerateNewId();
                        singleMeeting.Instructors = new List<string>();

                        //foreach (var instructor in Instructor)
                        //{

                        //}

                        //todo InstructorFindBySomething for example Name
                        // --> maybe 2 collections in ViewModel - firstName, LastName of all Instructors available
                        //todo Instructor in singleMeeting.Add(instructor.Id)
                        //todo Meeting add to collection in Mongo
                        //todo Add reference to meeting to Course.Meetings Array

                        //_context.AddMeeting(singleMeeting);
                    }
                }

                if (course.DateOfEnd != null)
                {
                    course.CourseLength = course.DateOfEnd.Subtract(course.DateOfStart).Days;
                }

                _context.courseRepository.AddCourse(course);

                return RedirectToAction("ConfirmationOfActionOnCourse", new { courseIdentificator = course.CourseIdentificator, meetingsIdentificators = new List<string>(), TypeOfAction = "Update" });
            }

            newCourse.AvailableBranches = _context.branchRepository.GetBranchesAsSelectList().ToList();
            if (newCourse.SelectedBranches == null)
            {
                newCourse.SelectedBranches = new List<string>();
            }

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
        [Authorize(Roles = "Admin")]
        public ActionResult CourseDetails(string courseIdentificator, string message)
        {
            ViewBag.Message = message;

            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            List<DisplayMeetingViewModel> meetingsViewModel = new List<DisplayMeetingViewModel>();

            // przemyśleć nad zmniejszeniem ViewModelu -> niepotrzebne pola
            foreach (var meeting in Meetings)
            {
                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.CourseIdentificator = Course.CourseIdentificator;
                singleMeeting.InstructorsCredentials = _context.userRepository.GetInstructorsById(meeting.Instructors).Select(z => z.FirstName + " " + z.LastName).ToList();

                meetingsViewModel.Add(singleMeeting);
            }

            var Users = _context.userRepository.GetUsersById(Course.EnrolledUsers);

            List<DisplayCrucialDataWithCompaniesRoleUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataWithCompaniesRoleUserViewModel>>(Users);
            usersViewModel.ForEach(z => z.CompanyRoleManager = _context.companyRepository.GetCompaniesById(z.CompanyRoleManager).Select(s => s.CompanyName).ToList());
            usersViewModel.ForEach(z => z.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(z.CompanyRoleWorker).Select(s => s.CompanyName).ToList());

            var InstructorsIdentificators = new List<string>();
            Meetings.ToList().ForEach(z => z.Instructors.ToList().ForEach(s => InstructorsIdentificators.Add(s)));

            var Instructors = _context.userRepository.GetUsersById(InstructorsIdentificators.Distinct().ToList());
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Instructors);

            var Exams = _context.examRepository.GetExamsById(Course.Exams);
            List<DisplayExamWithoutCourseViewModel> examsViewModel = _mapper.Map<List<DisplayExamWithoutCourseViewModel>>(Exams);

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

            CourseDetailsViewModel courseDetails = _mapper.Map<CourseDetailsViewModel>(Course);
            courseDetails.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            courseDetails.Meetings = meetingsViewModel;
            courseDetails.Exams = examsViewModel;

            courseDetails.EnrolledUsersQuantity = Course.EnrolledUsers.Count;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;
            courseDetails.Examiners = examinersViewModel;

            courseDetails.DispensedGivenCertificates = _mapper.Map<DispenseGivenCertificateCheckBoxViewModel[]>(usersViewModel);

            if (Course.CourseEnded)
            {
                foreach (var user in Users)
                {
                    var UsersGivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

                    if (UsersGivenCertificates.Where(z => z.Course == Course.CourseIdentificator).Count()!= 0)
                    {
                        courseDetails.DispensedGivenCertificates.Where(z => z.UserIdentificator == user.Id).Select(z => z.GivenCertificateIsEarned = true);
                    }
                 }
            }
      
            return View(courseDetails);
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
            usersAssignedToCourse.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(usersAssignedToCourse);
        }

        // GET: EndCourseAndDispenseGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult EndCourseAndDispenseGivenCertificates(string courseIdentificator)
        {
            var Course = _context.courseRepository.GetCourseById(courseIdentificator);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            if (Course.CourseEnded != true)
            {
                var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);

                List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

                if (EnrolledUsersList.Count != 0)
                {
                    ListOfUsers = GetCourseListOfUsersWithStatistics(EnrolledUsersList, Meetings).ToList();
                }

                // if course ended by exam - get exam result and show

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
                            GivenCertificateIdentificator = _keyGenerator.GenerateNewGuid(),
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
            }

            var EnrolledUsersList = _context.userRepository.GetUsersById(Course.EnrolledUsers);
            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            if (EnrolledUsersList.Count != 0)
            {
                ListOfUsers = GetCourseListOfUsersWithStatistics(EnrolledUsersList, Meetings).ToList();
            }

            courseToEndViewModel.AllCourseParticipants = ListOfUsers;
            courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

            return View(courseToEndViewModel);
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

                deleteUsersFromCourseViewModel.UsersToDeleteFromCourse = _mapper.Map<DeleteUsersFromCourseCheckBoxViewModel[]>(ListOfUsers);

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
                var UsersToDeleteFromCourseIdentificators = deleteUsersFromCourseViewModel.UsersToDeleteFromCourse.ToList().Where(z => z.IsToDeleteFromCourse == true).Select(z => z.UserIdentificator).ToList();

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

        #region HelpersMethod
        // GetCourseListOfUsersWithStatistics
        private ICollection<DisplayUserWithCourseResultsViewModel> GetCourseListOfUsersWithStatistics(ICollection<CertificationPlatformUser> enrolledUsers, ICollection<Meeting> meetings)
        {
            List<DisplayUserWithCourseResultsViewModel> ListOfUsers = new List<DisplayUserWithCourseResultsViewModel>();

            foreach (var user in enrolledUsers)
            {
                DisplayUserWithCourseResultsViewModel singleUser = _mapper.Map<DisplayUserWithCourseResultsViewModel>(user);

                // until Exam collection will be ready

                //singleUser.ExamPercentResult = 
                //singleUser.ExamAttempsQuantity = 

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

                ListOfUsers.Add(singleUser);
            }

            return ListOfUsers;
        }
        #endregion
    }
}