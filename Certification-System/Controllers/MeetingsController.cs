using Certification_System.DAL;
using Certification_System.Entitities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Certification_System.Controllers
{
    public class MeetingsController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public MeetingsController()
        {
            _context = new MongoOperations();
        }

        // GET: AddNewMeetingConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewMeetingConfirmation(string meetingIdentificator)
        {
            if (meetingIdentificator != null)
            {
                var Meeting = _context.GetMeetingById(meetingIdentificator);

                AddMeetingViewModel addedMeeting = new AddMeetingViewModel
                {
                    MeetingIdentificator = Meeting.MeetingIdentificator,

                    SelectedCourse = _context.GetCourseByMeetingId(meetingIdentificator).CourseIndexer,
                    MeetingIndexer = Meeting.MeetingIndexer,
                    Description = Meeting.Description,
                    DateOfMeeting = Meeting.DateOfMeeting,
                    Country = Meeting.Country,
                    City = Meeting.City,
                    PostCode = Meeting.PostCode,
                    Address = Meeting.Address,
                    NumberOfApartment = Meeting.NumberOfApartment,
                    SelectedInstructors = new List<string>()
                };

                if (addedMeeting.SelectedInstructors != null)
                {
                    var Instructors = _context.GetInstructorsById(Meeting.Instructors);
                    addedMeeting.SelectedInstructors = Instructors.Select(z => z.FirstName + " " +  z.LastName).ToList();
                }

                return View(addedMeeting);
            }

            return RedirectToAction(nameof(AddNewMeeting));
        }

        // LEGACY
        // GET: AddNewMeetingPartial
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewMeetingPartial()
        {
            return PartialView();
        }

        // GET: AddNewMeeting
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewMeeting()
        {
            AddMeetingViewModel newMeeting = new AddMeetingViewModel
            {
                AvailableCourses = _context.GetCoursesAsSelectList().ToList(),
                AvailableInstructors = _context.GetInstructorsAsSelectList().ToList()
            };

            return View(newMeeting);
        }

        // POST: AddNewMeeting
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNewMeeting(AddMeetingViewModel newMeeting)
        {
            if (ModelState.IsValid)
            {
                Meeting meeting = new Meeting
                {
                    MeetingIdentificator = ObjectId.GenerateNewId().ToString(),

                    MeetingIndexer = newMeeting.MeetingIndexer,
                    Description = newMeeting.Description,
                    DateOfMeeting = newMeeting.DateOfMeeting,
                    Country = newMeeting.Country,
                    City = newMeeting.City,
                    PostCode = newMeeting.PostCode,
                    Address = newMeeting.Address,
                    NumberOfApartment = newMeeting.NumberOfApartment,

                    Instructors = newMeeting.SelectedInstructors
                };

                _context.AddMeeting(meeting);
                _context.AddMeetingToCourse(meeting.MeetingIdentificator, newMeeting.SelectedCourse);

                return RedirectToAction("AddNewMeetingConfirmation", new { meetingIdentificator = meeting.MeetingIdentificator, TypeOfAction = "Add" });
            }

            newMeeting.AvailableCourses = _context.GetCoursesAsSelectList().ToList();
            newMeeting.AvailableInstructors = _context.GetInstructorsAsSelectList().ToList();
            return View(newMeeting);
        }


        // GET: EditMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult EditMeeting(string meetingIdentificator)
        {
          var Meeting = _context.GetMeetingById(meetingIdentificator);

            EditMeetingViewModel meetingViewModel = new EditMeetingViewModel
            {
                MeetingIdentificator = Meeting.MeetingIdentificator,

                MeetingIndexer = Meeting.MeetingIndexer,
                Description = Meeting.Description,
                DateOfMeeting = Meeting.DateOfMeeting,
                Country = Meeting.Country,
                City = Meeting.City,
                Address = Meeting.Address,
                NumberOfApartment = Meeting.NumberOfApartment,
                PostCode = Meeting.PostCode,

                AvailableInstructors = _context.GetInstructorsAsSelectList().ToList(),
                SelectedInstructors = Meeting.Instructors
            };

            return View(meetingViewModel);
        }

        // POST: EditMeeting
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditMeeting(EditMeetingViewModel editedMeeting)
        {
            if (ModelState.IsValid)
            {
                var OriginMeeting = _context.GetMeetingById(editedMeeting.MeetingIdentificator);

                OriginMeeting.MeetingIndexer = editedMeeting.MeetingIndexer;
                OriginMeeting.Description = editedMeeting.Description;
                OriginMeeting.DateOfMeeting = editedMeeting.DateOfMeeting;
                OriginMeeting.Country = editedMeeting.Country;
                OriginMeeting.City = editedMeeting.City;
                OriginMeeting.PostCode = editedMeeting.PostCode;
                OriginMeeting.NumberOfApartment = editedMeeting.NumberOfApartment;
                OriginMeeting.Address = editedMeeting.Address;
                OriginMeeting.Instructors = editedMeeting.SelectedInstructors;

                _context.UpdateMeeting(OriginMeeting);

                return RedirectToAction("AddNewMeetingConfirmation", "Meetings", new { meetingIdentificator = editedMeeting.MeetingIdentificator, TypeOfAction = "Update" });
            }

            return View(editedMeeting);
        }

        // GET: DisplayAllMeetings
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllMeetings()
        {
            var Meetings = _context.GetMeetings();
            List<DisplayMeetingViewModel> ListOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in Meetings)
            {
                Course RelatedCourse = _context.GetCourseByMeetingId(meeting.MeetingIdentificator);
                List<Instructor> RelatedInstructors = _context.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = new DisplayMeetingViewModel
                {
                    MeetingIdentificator = meeting.MeetingIdentificator,

                    MeetingIndexer = meeting.MeetingIndexer,
                    Description = meeting.Description,
                    DateOfMeeting = meeting.DateOfMeeting,
                    Country = meeting.Country,
                    City = meeting.City,
                    Address = meeting.Address,
                    NumberOfApartment = meeting.NumberOfApartment,
                    PostCode = meeting.PostCode,

                    CourseIdentificator = RelatedCourse.CourseIdentificator,
                    CourseIndexer = RelatedCourse.CourseIndexer,
                    CourseName = RelatedCourse.Name,

                    InstructorsIdentificators = RelatedInstructors.Select(z=> z.InstructorIdentificator).ToList(),
                    InstructorsCredentials = RelatedInstructors.Select(z => z.FirstName + " " + z.LastName).ToList()
                };

                ListOfMeetings.Add(singleMeeting);
            }

            return View(ListOfMeetings);
        }

        // GET: MeetingDetails
        [Authorize(Roles = "Admin")]
        public ActionResult MeetingDetails(string meetingIdentificator)
        {
            var Meeting = _context.GetMeetingById(meetingIdentificator);
            var RelatedInstructors = _context.GetInstructorsById(Meeting.Instructors);
            var EnrolledUsersIdentificators = _context.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;

            var EnrolledUsersList = _context.GetUsersById(EnrolledUsersIdentificators);

            List<AddInstructorViewModel> ListOfInstructors = new List<AddInstructorViewModel>();

            if (RelatedInstructors.Count != 0)
            {  
                foreach (var instructor in RelatedInstructors)
                {
                    AddInstructorViewModel singleInstructor = new AddInstructorViewModel
                    {
                        FirstName = instructor.FirstName,
                        LastName = instructor.LastName,
                        Email = instructor.Email,
                        Phone = instructor.Phone,
                        Country = instructor.Country,
                        City = instructor.City,
                        PostCode = instructor.PostCode,
                        Address = instructor.Address,
                        NumberOfApartment = instructor.NumberOfApartment
                    };

                    ListOfInstructors.Add(singleInstructor);
                }
            }
    
            List<DisplayUsersViewModel> ListOfUsers = new List<DisplayUsersViewModel>();

            if (EnrolledUsersList.Count != 0)
            {
                foreach (var user in EnrolledUsersList)
                {
                    DisplayUsersViewModel singleUser = new DisplayUsersViewModel
                    {
                        UserIdentificator = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                    };

                    ListOfUsers.Add(singleUser);
                }
            }

            MeetingDetailsViewModel MeetingDetails = new MeetingDetailsViewModel
            {
                MeetingIdentificator = Meeting.MeetingIdentificator,

                MeetingIndexer = Meeting.MeetingIndexer,
                Description = Meeting.Description,
                DateOfMeeting = Meeting.DateOfMeeting,
                Country = Meeting.Country,
                City = Meeting.City,
                Address = Meeting.Address,
                NumberOfApartment = Meeting.NumberOfApartment,
                PostCode = Meeting.PostCode,

                Instructors = ListOfInstructors,
                AttendanceList = Meeting.AttendanceList,
                AllCourseParticipants = ListOfUsers
            };

            return View(MeetingDetails);
        }
    }
}