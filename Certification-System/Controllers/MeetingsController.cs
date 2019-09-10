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
                var Course = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator);

                DisplayMeetingViewModel modifiedMeeting = _mapper.Map<DisplayMeetingViewModel>(Meeting);
                modifiedMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);

                if (Meeting.Instructors.Count() != 0)
                {
                    var Instructors = _context.userRepository.GetInstructorsById(Meeting.Instructors);
                    modifiedMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Instructors);
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
                AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList(),
                AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList()
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

            newMeeting.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            newMeeting.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();

            return View(newMeeting);
        }

        // GET: EditMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult EditMeeting(string meetingIdentificator)
        {
            var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

            EditMeetingViewModel meetingViewModel = _mapper.Map<EditMeetingViewModel>(Meeting);
            meetingViewModel.AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList();

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
            var Meetings = _context.meetingRepository.GetListOfMeetings();
            List<DisplayMeetingViewModel> ListOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in Meetings)
            {
                var Course = _context.courseRepository.GetCourseByMeetingId(meeting.MeetingIdentificator);
                var Instructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Instructors);

                ListOfMeetings.Add(singleMeeting);
            }

            return View(ListOfMeetings);
        }

        // GET: MeetingDetails
        [Authorize(Roles = "Admin")]
        public ActionResult MeetingDetails(string meetingIdentificator, bool checkedPresence)
        {
            ViewBag.CheckedPresence = checkedPresence;

            var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);
            var RelatedInstructors = _context.userRepository.GetInstructorsById(Meeting.Instructors);
            
            var EnrolledUsersIdentificators = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;
            var EnrolledUsersList = _context.userRepository.GetUsersById(EnrolledUsersIdentificators);

            List<DisplayCrucialDataWithContactUserViewModel> ListOfInstructors = new List<DisplayCrucialDataWithContactUserViewModel>();

            if (RelatedInstructors.Count != 0)
            {
               ListOfInstructors = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(RelatedInstructors);
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

        // GET: CheckUsersPresence
        [Authorize(Roles = "Admin")]
        public ActionResult CheckUsersPresence(string meetingIdentificator)
        {
            var Meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

            var EnrolledUsersIdentificators = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;
            var EnrolledUsersList = _context.userRepository.GetUsersById(EnrolledUsersIdentificators);

            List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

            if (EnrolledUsersList.Count != 0)
            {
                ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(EnrolledUsersList);
            }

            CheckMeetingPresenceViewModel MeetingPresence = _mapper.Map<CheckMeetingPresenceViewModel>(Meeting);

            MeetingPresence.AttendanceList = _mapper.Map<PresenceCheckBoxViewModel[]>(ListOfUsers);
            MeetingPresence.AttendanceList.ToList().ForEach(z => z.IsPresent = Meeting.AttendanceList.Contains(z.UserIdentificator));

            MeetingPresence.AllCourseParticipants = ListOfUsers;

            return View(MeetingPresence);
        }

        // POST: CheckUsersPresence
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CheckUsersPresence(CheckMeetingPresenceCrucialDataViewModel meetingWithPresenceToCheck)
        {
            var PresentUsersIdentificators = meetingWithPresenceToCheck.AttendanceList.ToList().Where(z => z.IsPresent == true).Select(z => z.UserIdentificator).ToList();
            _context.meetingRepository.ChangeUsersPresenceOnMeetings(meetingWithPresenceToCheck.MeetingIdentificator, PresentUsersIdentificators);

            return RedirectToAction("MeetingDetails", "Meetings", new { meetingIdentificator = meetingWithPresenceToCheck.MeetingIdentificator, checkedPresence = true });
        }

        // GET: InstructorMeetings
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorMeetings()
        {
            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Meetings = _context.meetingRepository.GetMeetingsByInstructorId(User.Id);
            List<DisplayMeetingViewModel> ListOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in Meetings)
            {
                var Course = _context.courseRepository.GetCourseByMeetingId(meeting.MeetingIdentificator);
                var Instructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Instructors);

                ListOfMeetings.Add(singleMeeting);
            }

            return View(ListOfMeetings);
        }
    }
}