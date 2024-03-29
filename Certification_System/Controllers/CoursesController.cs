﻿using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Certification_System.Controllers
{
    public class CoursesController : Controller
    {
        public IDatabaseOperations _context { get; set; }

        public CoursesController()
        {
            _context = new MongoOperations();
        }

        // GET: AddNewCourse
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCourse()
        {
            AddCourseViewModel newCourse = new AddCourseViewModel
            {
                AvailableBranches = new List<SelectListItem>(),
                SelectedBranches = new List<string>(),

                MeetingsViewModels = new List<AddMeetingViewModel>()
            };

            newCourse.AvailableBranches = _context.GetBranchesAsSelectList().ToList();

            //todo GetInstructorsData and store it in some collection in AddCourseViewModel

            return View(newCourse);
        }

        // GET: AddNewCourseConfirmation
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCourseConfirmation(string courseIdentificator, ICollection<string> MeetingsIdentificators)
        {
            if (courseIdentificator != null)
            {
                var Course = _context.GetCourseById(courseIdentificator);

                AddCourseViewModel addedCourse = new AddCourseViewModel
                {
                    CourseIndexer = Course.CourseIndexer,
                    Name = Course.Name,
                    Description = Course.Description,
                    DateOfStart = Course.DateOfStart,
                    DateOfEnd = Course.DateOfEnd,

                    MeetingsViewModels = new List<AddMeetingViewModel>()
                };

                if (Course.Meetings != null)
                {
                    var MeetingsId = _context.GetMeetingsById(Course.Meetings);
                    List<AddMeetingViewModel> Meetings = new List<AddMeetingViewModel>();

                    foreach (var meeting in MeetingsId)
                    {
                        var Instructors = _context.GetInstructorsById(meeting.Instructor);

                        AddMeetingViewModel meetingsInCourse = new AddMeetingViewModel
                        {
                            MeetingIndexer = meeting.MeetingIndexer,
                            Description = meeting.Description,
                            DateOfMeeting = meeting.DateOfMeeting,
                            Country = meeting.Country,
                            City = meeting.City,
                            PostCode = meeting.PostCode,
                            Address = meeting.Address,
                            NumberOfApartment = meeting.NumberOfApartment,

                            Instructors = new List<string>()
                        };

                        foreach (var instructor in Instructors)
                        {
                            string instructorIdentity = instructor.FirstName + instructor.LastName;
                            meetingsInCourse.Instructors.Add(instructorIdentity);
                        }

                        Meetings.Add(meetingsInCourse);
                    }

                    addedCourse.MeetingsViewModels = Meetings;
                }

                var BranchNames = _context.GetBranchesById(Course.Branches);
                addedCourse.SelectedBranches = BranchNames;

                return View(addedCourse);
            }
            return RedirectToAction(nameof(AddNewCourse));
        }

        // POST: AddNewCourse
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewCourse(AddCourseViewModel newCourse)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseIdentificator = ObjectId.GenerateNewId().ToString(),

                    Name = newCourse.Name,
                    Description = newCourse.Description,
                    CourseIndexer = newCourse.CourseIndexer,
                    DateOfStart = newCourse.DateOfStart,
                    DateOfEnd = newCourse.DateOfEnd,
                    EnrolledUsersLimit = newCourse.EnrolledUsersLimit,

                    CourseEnded = false,
                    Branches = newCourse.SelectedBranches,
                    Meetings = new List<string>()
                };

                if (newCourse.MeetingsViewModels != null)
                {
                    foreach (var meeting in newCourse.MeetingsViewModels)
                    {
                        Meeting singleMeeting = new Meeting
                        {
                            MeetingIdentificator = ObjectId.GenerateNewId().ToString(),

                            MeetingIndexer = meeting.MeetingIndexer,
                            Description = meeting.Description,
                            DateOfMeeting = meeting.DateOfMeeting,
                            Country = meeting.Country,
                            City = meeting.City,
                            PostCode = meeting.PostCode,
                            Address = meeting.Address,
                            NumberOfApartment = meeting.NumberOfApartment,

                            Instructor = new List<string>()
                        };

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

                _context.AddCourse(course);

                return RedirectToAction("AddNewCourseConfirmation", new { courseIdentificator = course.CourseIdentificator, MeetingsIdentificators = new List<string>() });
            }

            newCourse.AvailableBranches = _context.GetBranchesAsSelectList().ToList();
            if (newCourse.SelectedBranches == null)
            {
                newCourse.SelectedBranches = new List<string>();
            }
            return View(newCourse);
        }

    }
}