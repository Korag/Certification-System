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
            GetCourses();
            _courses.InsertOne(course);
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
            ICollection<Course> resultListOfCourses = GetCourses().Find<Course>(filter).ToList();

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

        public Course GetCourseByMeetingId(string meetingIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Meetings.Contains(meetingIdentificator));
            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();

            return resultCourse;
        }

        public ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators)
        {
            GetCourses();
            ICollection<Course> Courses = new List<Course>();

            foreach (var courseIdentificator in coursesIdentificators)
            {
                var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
                var singleCourse = _courses.Find<Course>(filter).FirstOrDefault();
                Courses.Add(singleCourse);
            }

            return Courses;
        }

        public void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator)
        {
            var course = GetCourseById(courseIdentificator);
            course.Meetings.Add(meetingIdentificator);

            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);

            GetCourses().ReplaceOne(filter, course);
        }
    }
}
