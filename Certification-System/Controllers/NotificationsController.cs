﻿using AutoMapper;
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
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public NotificationsController(
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

        // GET: NotificationManagerHub
        [Authorize]
        public ActionResult NotificationManagerHub()
        {
            if (this.User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminNotificationManager", "Notifications");
            }
            else if (this.User.IsInRole("Worker"))
            {
                return RedirectToAction("WorkerNotificationManager", "Notifications");
            }
            else if (this.User.IsInRole("Company"))
            {
                return RedirectToAction("CompanyNotificationManager", "Notifications");
            }
            else if(this.User.IsInRole("Instructor") && this.User.IsInRole("Examiner"))
            {
                return RedirectToAction("InstructorExaminerNotificationManager", "Notifications");
            }
            else if (this.User.IsInRole("Instructor"))
            {
                return RedirectToAction("InstructorNotificationManager", "Notifications");
            }
            else if (this.User.IsInRole("Examiner"))
            {
                return RedirectToAction("ExaminerNotificationManager", "Notifications");
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

        // GET: InstructorNotificationManager
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: ExaminerNotificationManager
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: InstructorExaminerNotificationManager
        [Authorize(Roles = "Instructor")]
        [Authorize(Roles = "Examiner")]
        public ActionResult InstructorExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: CompanyNotificationManager
        [Authorize(Roles = "Company")]
        public ActionResult CompanyExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: WorkerNotificationManager
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }
    }
}