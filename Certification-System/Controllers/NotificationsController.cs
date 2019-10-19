using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Certification_System.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;

        public NotificationsController(
               MongoOperations context,
               IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: NotificationManagerHub
        [Authorize]
        public ActionResult NotificationManagerHub()
        {
            if (this.User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminNotificationManager", "Notifications");
            }
            else if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("NotificationManager", "Notifications");
            }
        
            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: AdminNotificationManager
        [Authorize(Roles = "Admin")]
        public ActionResult AdminNotificationManager(string message = null)
        {
            ViewBag.message = message;

            var coursesQueue = _context.courseRepository.GetListOfCoursesQueue();

            List<CourseQueueNotificationViewModel> notificationsCoursesQueue = new List<CourseQueueNotificationViewModel>();

            foreach (var courseQueue in coursesQueue)
            {
                var course = _context.courseRepository.GetCourseById(courseQueue.CourseIdentificator);

                foreach (var awaitingUser in courseQueue.AwaitingUsers)
                {
                    var user = _context.userRepository.GetUserById(awaitingUser.UserIdentificator);

                    CourseQueueNotificationViewModel singleNotification = new CourseQueueNotificationViewModel
                    {
                        Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course),
                        EnrolledUser = _mapper.Map<DisplayCrucialDataUserViewModel>(user),


                        EnrollmentDate = awaitingUser.LogData.DateOfLogCreation,
                        EnrollmentOlderThan2Weeks = false
                    };

                    if (DateTime.Now.Subtract(singleNotification.EnrollmentDate).Days > 14)
                    {
                        singleNotification.EnrollmentOlderThan2Weeks = true;
                    }

                    notificationsCoursesQueue.Add(singleNotification);
                }
            }

            var notEndedCoursesAfterEndDate = _context.courseRepository.GetCoursesAfterEndDate();
            List<DisplayCourseNotificationViewModel> notificationsNotEndedCourses = new List<DisplayCourseNotificationViewModel>();

            foreach (var notEndedCourse in notEndedCoursesAfterEndDate)
            {
                DisplayCourseNotificationViewModel singleNotification = _mapper.Map<DisplayCourseNotificationViewModel>(notEndedCourse);

                notificationsNotEndedCourses.Add(singleNotification);
            }

            AdminNotificationViewModel adminNotifications = new AdminNotificationViewModel
            {
                CourseQueueNotification = notificationsCoursesQueue,
                NotEndedCoursesAfterEndDate = notificationsNotEndedCourses
            };

            return View(adminNotifications);
        }

        // GET: NotificationManager
        [Authorize]
        public ActionResult NotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var userLogs = _context.personalLogRepository.GetPersonalUserLogById(user.Id);

            List<DisplayLogInformationViewModel> userPersonalLogs = new List<DisplayLogInformationViewModel>();

            if (userLogs != null)
            {
                foreach (var userLog in userLogs.LogData)
                {
                    DisplayLogInformationViewModel singleLog= _mapper.Map<DisplayLogInformationViewModel>(userLog);

                    var changeAuthor = _context.userRepository.GetUserById(userLog.ChangeAuthorIdentificator);
                    singleLog.ChangeAuthor = _mapper.Map<DisplayCrucialDataUserViewModel>(changeAuthor);

                    userPersonalLogs.Add(singleLog);
                }
            }

            return View(userPersonalLogs);
        }
    }
}