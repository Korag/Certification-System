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
        public ActionResult CourseDetails(string courseIdentificator, bool addedNewUsersToCourse)
        {
            ViewBag.AddedNewUsersToCourse = addedNewUsersToCourse;

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

            CourseDetailsViewModel courseDetails = _mapper.Map<CourseDetailsViewModel>(Course);

            courseDetails.EnrolledUsersQuantity = Course.EnrolledUsers.Count;
            courseDetails.Branches = _context.branchRepository.GetBranchesById(Course.Branches);
            courseDetails.Meetings = meetingsViewModel;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.EnrolledUsers = usersViewModel;
            courseDetails.Instructors = instructorsViewModel;

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

                        return RedirectToAction("CourseDetails", new { courseIdentificator = usersAssignedToCourse.SelectedCourse, addedNewUsersToCourse = true });
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
                    foreach (var user in EnrolledUsersList)
                    {
                        DisplayUserWithCourseResultsViewModel singleUser = _mapper.Map<DisplayUserWithCourseResultsViewModel>(user);

                        // until Exam collection will be ready

                        //singleUser.ExamPercentResult = 
                        //singleUser.ExamAttempsQuantity = 

                        try
                        {
                            double UserPresencePercentage = ((Meetings.Where(z => z.AttendanceList.Contains(singleUser.UserIdentificator)).Count() / Meetings.Count()) * 100);
                            UserPresencePercentage = Math.Round(UserPresencePercentage, 2);
                            singleUser.PercentageOfUserPresenceOnMeetings = UserPresencePercentage;
                        }
                        catch (Exception e)
                        {
                            singleUser.PercentageOfUserPresenceOnMeetings = 0.00;
                        }

                        ListOfUsers.Add(singleUser);
                    }
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
            if (ModelState.IsValid)
            {
                var Course = _context.courseRepository.GetCourseById(courseToEndViewModel.CourseIdentificator);
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
                            //Certificate = Course.Ce
                        };

                        _context.givenCertificateRepository.AddGivenCertificate(singleGivenCertificate);

                        var User = _context.userRepository.GetUserById(courseToEndViewModel.DispensedGivenCertificates[i].UserIdentificator);
                        User.GivenCertificates.Add(singleGivenCertificate.GivenCertificateIdentificator);

                        _context.userRepository.UpdateUser(User);
                    }
                }
            }

            //courseToEndViewModel.AllCourseParticipants = ListOfUsers;
            courseToEndViewModel.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();

            return View(courseToEndViewModel);
        }
    }
}