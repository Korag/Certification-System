using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
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

                return RedirectToAction("AddNewMeetingConfirmation", new { meetingIdentificator = meeting.MeetingIdentificator });
            }

            newMeeting.AvailableCourses = _context.GetCoursesAsSelectList().ToList();
            newMeeting.AvailableInstructors = _context.GetInstructorsAsSelectList().ToList();
            return View(newMeeting);
        }
    }
}