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
            }

            return View(ListOfCourses);
        }


        // GET: CourseDetails
        [Authorize(Roles = "Admin")]
        public ActionResult CourseDetails(string CourseIdentificator)
        {
            var Course = _context.courseRepository.GetCourseById(CourseIdentificator);
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

        [Authorize(Roles = "Admin")]
        public ActionResult Test()
        {
            return Ok();
        }
    }
}