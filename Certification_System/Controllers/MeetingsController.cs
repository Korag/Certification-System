using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult AddNewMeetingConfirmation(string meetingIdentificator)
        {
            return View();
        }

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
                AvailableCourses = _context.GetCoursesAsSelectList().ToList()
            };

            return View(newMeeting);
        }

        // POST: AddNewMeeting
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewMeeting(AddMeetingViewModel newMeeting)
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