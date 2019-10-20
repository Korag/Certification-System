using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using AutoMapper;
using Certification_System.Extensions;

namespace Certification_System.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public MeetingsController(
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

        // GET: ConfirmationOfActionOnMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnMeeting(string meetingIdentificator)
        {
            if (meetingIdentificator != null)
            {
                var meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);
                var course = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator);

                DisplayMeetingViewModel modifiedMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                modifiedMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                if (meeting.Instructors.Count() != 0)
                {
                    var instructors = _context.userRepository.GetInstructorsById(meeting.Instructors);
                    modifiedMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors);
                }

                return View(modifiedMeeting);
            }

            return RedirectToAction(nameof(AddNewMeeting));
        }

        // GET: AddNewMeeting
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewMeeting(string courseIdentificator)
        {
            AddMeetingViewModel newMeeting = new AddMeetingViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList(),
                AvailableInstructors = _context.userRepository.GetInstructorsAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newMeeting.SelectedCourse = courseIdentificator;
            }

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

                var course = _context.courseRepository.GetCourseById(newMeeting.SelectedCourse);
                meeting.MeetingIndexer = _keyGenerator.GenerateMeetingEntityIndexer(course.CourseIndexer);

                _context.meetingRepository.AddMeeting(meeting);
                _context.courseRepository.AddMeetingToCourse(meeting.MeetingIdentificator, newMeeting.SelectedCourse);

                var logInfoAddMeeting = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addMeeting"]);
                _logger.AddMeetingLog(meeting, logInfoAddMeeting);

                var updatedCourse = _context.courseRepository.GetCourseById(newMeeting.SelectedCourse);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addMeetingToCourse"]);
                _logger.AddCourseLog(updatedCourse, logInfoUpdateCourse);

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
            var meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

            EditMeetingViewModel meetingViewModel = _mapper.Map<EditMeetingViewModel>(meeting);
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
                var originMeeting = _context.meetingRepository.GetMeetingById(editedMeeting.MeetingIdentificator);
                originMeeting = _mapper.Map<EditMeetingViewModel, Meeting>(editedMeeting, originMeeting);

                _context.meetingRepository.UpdateMeeting(originMeeting);

                var logInfoUpdateMeeting = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateMeeting"]);
                _logger.AddMeetingLog(originMeeting, logInfoUpdateMeeting);

                return RedirectToAction("ConfirmationOfActionOnMeeting", "Meetings", new { meetingIdentificator = editedMeeting.MeetingIdentificator, TypeOfAction = "Update" });
            }

            return View(editedMeeting);
        }

        // GET: DisplayAllMeetings
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllMeetings(string message = null)
        {
            ViewBag.Message = message;

            var meetings = _context.meetingRepository.GetListOfMeetings();
            List<DisplayMeetingViewModel> listOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in meetings)
            {
                var course = _context.courseRepository.GetCourseByMeetingId(meeting.MeetingIdentificator);
                var instructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors);

                listOfMeetings.Add(singleMeeting);
            }

            return View(listOfMeetings);
        }

        // GET: MeetingDetails
        [Authorize(Roles = "Admin, Instructor")]
        public ActionResult MeetingDetails(string meetingIdentificator, bool checkedPresence)
        {
            ViewBag.CheckedPresence = checkedPresence;

            var meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);
            var relatedInstructors = _context.userRepository.GetInstructorsById(meeting.Instructors);
            
            var enrolledUsersIdentificators = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;
            var enrolledUsersList = _context.userRepository.GetUsersById(enrolledUsersIdentificators);

            List<DisplayCrucialDataWithContactUserViewModel> listOfInstructors = new List<DisplayCrucialDataWithContactUserViewModel>();

            if (relatedInstructors.Count != 0)
            {
                listOfInstructors = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(relatedInstructors);
            }

            List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

            if (enrolledUsersList.Count != 0)
            {
                ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledUsersList);
            }

            MeetingDetailsViewModel MeetingDetails = _mapper.Map<MeetingDetailsViewModel>(meeting);
            MeetingDetails.Instructors = listOfInstructors;
            MeetingDetails.AttendanceList = meeting.AttendanceList;
            MeetingDetails.AllCourseParticipants = ListOfUsers;

            if (this.User.IsInRole("Instructor"))
            {
                return View("InstructorMeetingDetails", MeetingDetails);
            }

            return View(MeetingDetails);
        }

        // GET: CheckUsersPresence
        [Authorize(Roles = "Admin, Instructor")]
        public ActionResult CheckUsersPresence(string meetingIdentificator)
        {
            var meeting = _context.meetingRepository.GetMeetingById(meetingIdentificator);

            var enrolledUsersIdentificators = _context.courseRepository.GetCourseByMeetingId(meetingIdentificator).EnrolledUsers;
            var enrolledUsersList = _context.userRepository.GetUsersById(enrolledUsersIdentificators);

            List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

            if (enrolledUsersList.Count != 0)
            {
                listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledUsersList);
            }

            CheckMeetingPresenceViewModel MeetingPresence = _mapper.Map<CheckMeetingPresenceViewModel>(meeting);

            MeetingPresence.AttendanceList = _mapper.Map<PresenceCheckBoxViewModel[]>(listOfUsers);
            MeetingPresence.AttendanceList.ToList().ForEach(z => z.IsPresent = meeting.AttendanceList.Contains(z.UserIdentificator));

            MeetingPresence.AllCourseParticipants = listOfUsers;

            return View(MeetingPresence);
        }

        // POST: CheckUsersPresence
        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public ActionResult CheckUsersPresence(CheckMeetingPresenceCrucialDataViewModel meetingWithPresenceToCheck)
        {
            var presentUsersIdentificators = meetingWithPresenceToCheck.AttendanceList.ToList().Where(z => z.IsPresent == true).Select(z => z.UserIdentificator).ToList();
            _context.meetingRepository.ChangeUsersPresenceOnMeetings(meetingWithPresenceToCheck.MeetingIdentificator, presentUsersIdentificators);

            var updatedMeeting = _context.meetingRepository.GetMeetingById(meetingWithPresenceToCheck.MeetingIdentificator);

            var logInfoCheckPresence = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["checkPresenceOnMeeting"]);
            _logger.AddMeetingLog(updatedMeeting, logInfoCheckPresence);

            return RedirectToAction("MeetingDetails", "Meetings", new { meetingIdentificator = meetingWithPresenceToCheck.MeetingIdentificator, checkedPresence = true });
        }

        // GET: InstructorMeetings
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorMeetings()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var meetings = _context.meetingRepository.GetMeetingsByInstructorId(user.Id);
            List<DisplayMeetingViewModel> listOfMeetings = new List<DisplayMeetingViewModel>();

            foreach (var meeting in meetings)
            {
                var course = _context.courseRepository.GetCourseByMeetingId(meeting.MeetingIdentificator);
                var instructors = _context.userRepository.GetInstructorsById(meeting.Instructors).ToList();

                DisplayMeetingViewModel singleMeeting = _mapper.Map<DisplayMeetingViewModel>(meeting);
                singleMeeting.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors);

                listOfMeetings.Add(singleMeeting);
            }

            return View(listOfMeetings);
        }

        // GET: DeleteMeetingsHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteMeetingHub(string meetingIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(meetingIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteMeetingEntityLink(meetingIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteMeeting
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteMeeting(string meetingIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(meetingIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel meetingToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = meetingIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie spotkania w ramach kursu"
                };

                return View("DeleteEntity", meetingToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteMeeting
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteMeeting(DeleteEntityViewModel meetingToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var meeting = _context.meetingRepository.GetMeetingById(meetingToDelete.EntityIdentificator);

            if (meeting == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, meetingToDelete.Code))
            {
                var logInfoDeleteMeeting = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteMeeting"]);
                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeMeetingFromCourse"]);

                _context.meetingRepository.DeleteMeeting(meetingToDelete.EntityIdentificator);
                _logger.AddMeetingLog(meeting, logInfoDeleteMeeting);

                var updatedCourse = _context.courseRepository.DeleteMeetingFromCourse(meetingToDelete.EntityIdentificator);
                _logger.AddCourseLog(updatedCourse, logInfoUpdateCourse);

                return RedirectToAction("DisplayAllMeetings", "Meetings", new { message = "Usunięto wskazane spotkanie" });
            }

            return View("DeleteEntity", meetingToDelete);
        }
    }
}