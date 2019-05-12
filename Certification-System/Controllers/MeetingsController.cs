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
            return View();
        }

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
                AvailableCourses = _context.GetCoursesAsSelectList().ToList()
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

                    // instructors
                };

                _context.AddMeeting(meeting);

                return RedirectToAction("AddNewMeetingConfirmation", new { meetingIdentificator = meeting.MeetingIdentificator });
            }

            newMeeting.AvailableCourses = _context.GetCoursesAsSelectList().ToList();
            return View(newMeeting);
        }
    }
}