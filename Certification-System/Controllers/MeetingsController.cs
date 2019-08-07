using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using AutoMapper;

namespace Certification_System.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public MeetingsController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: ConfirmationOfActionOnMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnMeeting(string meetingIdentificator)
        {
            if (meetingIdentificator != null)
            {
                var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

                DisplayMeetingViewModel modifiedMeeting = _mapper.Map<DisplayMeetingViewModel>(Meeting);
                modifiedMeeting.CourseIndexer = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).CourseIndexer;
                modifiedMeeting.InstructorsCredentials = new List<string>();
          
                if (Meeting.Instructors != null)
                {
                    var Instructors = _context.instructorRepository.GetInstructorsById(Meeting.Instructors);
                    modifiedMeeting.InstructorsCredentials = Instructors.Select(z => z.FirstName + " " + z.LastName).ToList();
                }

                return View(modifiedMeeting);
            }

            return RedirectToAction(nameof(AddNewMeeting));
        }

        // LEGACY
        // GET: AddNewMeetingPartial
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewMeetingPartial()
        {
            return PartialView();
        }

        // GET: AddNewMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewMeeting()
        {
            AddMeetingViewModel newMeeting = new AddMeetingViewModel
            {
                AvailableCourses = _context.courseRepository.GetCoursesAsSelectList().ToList(),
                AvailableInstructors = _context.instructorRepository.GetInstructorsAsSelectList().ToList()
            };

            return View(newMeeting);
        }

        // POST: AddNewMeeting
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewMeeting(AddMeetingViewModel newMeeting)
        {
            if (ModelState.IsValid)
            {
                Meeting meeting = _mapper.Map<Meeting>(newMeeting);
                meeting.MeetingIdentificator = _keyGenerator.GenerateNewId();

                _context.meetingRepository.AddMeeting(meeting);
                _context.courseRepository.AddMeetingToCourse(meeting.MeetingIdentificator, newMeeting.SelectedCourse);

                return RedirectToAction("ConfirmationOfActionOnMeeting", new { meetingIdentificator = meeting.MeetingIdentificator, TypeOfAction = "Add" });
            }

            newMeeting.AvailableCourses = _context.courseRepository.GetCoursesAsSelectList().ToList();
            newMeeting.AvailableInstructors = _context.instructorRepository.GetInstructorsAsSelectList().ToList();

            return View(newMeeting);
        }

        // GET: EditMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult EditMeeting(string meetingIdentificator)
        {
            var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

            EditMeetingViewModel meetingViewModel = _mapper.Map<EditMeetingViewModel>(Meeting);
            meetingViewModel.AvailableInstructors = _context.instructorRepository.GetInstructorsAsSelectList().ToList();

            return View(meetingViewModel);
        }

        // POST: EditMeeting
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditMeeting(EditMeetingViewModel editedMeeting)
        {
            if (ModelState.IsValid)
            {
                var OriginMeeting = _context.meetingRepository.GetMeetingById(editedMeeting.MeetingIdentificator);
                OriginMeeting = _mapper.Map<EditMeetingViewModel, Meeting>(editedMeeting, OriginMeeting);

                _context.meetingRepository.UpdateMeeting(OriginMeeting);

                return RedirectToAction("ConfirmationOfActionOnMeeting", "Meetings", new { meetingIdentificator = editedMeeting.MeetingIdentificator, TypeOfAction = "Update" });
            }

            return View(editedMeeting);
        }

        // GET: DisplayAllMeetings
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllMeetings()
        {
            var Meetings = _context.meetingRepository.GetMeetings();
            List<DisplayMeetingViewModel> ListOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in Meetings)
            {
                Course RelatedCourse = _context.courseRepository.GetCourseByMeetingId(meeting.MeetingIdentificator);
                List<Instructor> RelatedInstructors = _context.instructorRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.CourseIdentificator = RelatedCourse.CourseIdentificator;
                singleMeeting.CourseIndexer = RelatedCourse.CourseIndexer;
                singleMeeting.CourseName = RelatedCourse.Name;

                singleMeeting.InstructorsIdentificators = RelatedInstructors.Select(z => z.InstructorIdentificator).ToList();
                singleMeeting.InstructorsCredentials = RelatedInstructors.Select(z => z.FirstName + " " + z.LastName).ToList();

                ListOfMeetings.Add(singleMeeting);
            }

            return View(ListOfMeetings);
        }

        // GET: MeetingDetails
        [Authorize(Roles = "Admin")]
        public ActionResult MeetingDetails(string meetingIdentificator)
        {
            var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);
            var RelatedInstructors = _context.instructorRepository.GetInstructorsById(Meeting.Instructors);
            
            var EnrolledUsersIdentificators = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;
            var EnrolledUsersList = _context.userRepository.GetUsersById(EnrolledUsersIdentificators);

            List<DisplayCrucialDataWithContactUsersViewModel> ListOfInstructors = new List<DisplayCrucialDataWithContactUsersViewModel>();

            if (RelatedInstructors.Count != 0)
            {
               ListOfInstructors = _mapper.Map<List<DisplayCrucialDataWithContactUsersViewModel>>(RelatedInstructors);
            }

            List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

            if (EnrolledUsersList.Count != 0)
            {
                ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(EnrolledUsersList);
            }

            MeetingDetailsViewModel MeetingDetails = _mapper.Map<MeetingDetailsViewModel>(Meeting);
            MeetingDetails.Instructors = ListOfInstructors;
            MeetingDetails.AttendanceList = Meeting.AttendanceList;
            MeetingDetails.AllCourseParticipants = ListOfUsers;

            return View(MeetingDetails);
        }
    }
}