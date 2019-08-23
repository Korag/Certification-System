using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly MongoContext _context;

        private readonly string _coursesCollectionName = "Courses";
        private IMongoCollection<Course> _courses;

        public CourseRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<Course> GetCourses()
        {
            return _courses = _context.db.GetCollection<Course>(_coursesCollectionName);
        }

        public ICollection<Course> GetListOfCourses()
        {
            return GetCourses().AsQueryable().ToList();
        }

        public void AddCourse(Course course)
        {
            GetCourses().InsertOne(course);
        }

        public void UpdateCourse(Course course)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, course.CourseIdentificator);
            var result = GetCourses().ReplaceOne(filter, course);
        }

        public Course GetCourseById(string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();

            return resultCourse;
        }

        public ICollection<Course> GetActiveCourses()
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseEnded, false);
            var resultListOfCourses = GetCourses().Find<Course>(filter).ToList();

            return resultListOfCourses;
        }

        public ICollection<SelectListItem> GetActiveCoursesAsSelectList()
        {
            List<Course> Courses = GetActiveCourses().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var course in Courses)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetActiveCoursesWithVacantSeatsAsSelectList()
        {
            List<Course> Courses = GetActiveCourses().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var course in Courses)
            {
                var VacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " " + course.Name + " |wm.: " + VacantSeats,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetAllCoursesAsSelectList()
        {
            GetCourses();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var course in _courses.AsQueryable().ToList())
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public void AddEnrolledUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Course>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var update = Builders<Course>.Update.AddToSetEach(x => x.EnrolledUsers, usersIdentificators);

            var result = GetCourses().UpdateOne(filter, update);
        }

        public Course GetCourseByMeetingId(string meetingIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Meetings.Contains(meetingIdentificator));
            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();

            return resultCourse;
        }

        public ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators)
        {
            var filter = Builders<Course>.Filter.Where(z => coursesIdentificators.Contains(z.CourseIdentificator));
            var resultListOfCourses = GetCourses().Find<Course>(filter).ToList();

            return resultListOfCourses;
        }

        public void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator)
        {
            var course = GetCourseById(courseIdentificator);
            course.Meetings.Add(meetingIdentificator);

            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);

            GetCourses().ReplaceOne(filter, course);
        }

        public ICollection<Course> GetCoursesByMeetingsId(ICollection<string> meetingsIdentificators)
        {
            var resultListOfCourses = GetCourses().AsQueryable().ToList().Where(z => z.Meetings.ToList().Where(x => meetingsIdentificators.Contains(x)).ToList().Count() != 0).ToList();

            return resultListOfCourses;
        }

        public void DeleteUsersFromCourse(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Course>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var update = Builders<Course>.Update.PullAll(x => x.EnrolledUsers, usersIdentificators);

            var result = GetCourses().UpdateOne(filter, update);
        }

        public ICollection<Course> GetExaminerCourses(string userIdentificator, ICollection<Exam> exams)
        {
            GetCourses();

            List<Course> resultListOfCourses = new List<Course>();

            foreach (var exam in exams)
            {
                resultListOfCourses.Add(_courses.AsQueryable().ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)).FirstOrDefault());
            }

            return resultListOfCourses;
        }

        public Course GetCourseByExamId(string examIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Exams.Contains(examIdentificator));
            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();

            return resultCourse;
        }
    }
}
