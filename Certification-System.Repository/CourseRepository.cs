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
        private MongoContext _context;

        private string _coursesCollectionName = "Courses";
        private IMongoCollection<Course> _courses;

        public CourseRepository(MongoContext context)
        {
            _context = context;
        }

        public void AddCourse(Course course)
        {
            _courses = _context.db.GetCollection<Course>(_coursesCollectionName);
            _courses.InsertOne(course);
        }

        public void UpdateCourse(Course course)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, course.CourseIdentificator);
            var result = _context.db.GetCollection<Course>(_coursesCollectionName).ReplaceOne(filter, course);
        }

        public Course GetCourseById(string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            return course;
        }

        public ICollection<Course> GetActiveCourses()
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseEnded, false);
            ICollection<Course> course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).ToList();
            return course;
        }

        public ICollection<Course> GetCourses()
        {
            _courses = _context.db.GetCollection<Course>(_coursesCollectionName);
            return _courses.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetCoursesAsSelectList()
        {
            List<Course> Courses = GetActiveCourses().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();
            //SelectList.Add(new SelectListItem { Text = "---", Value = null });

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

        public Course GetCourseByMeetingId(string meetingIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Meetings.Contains(meetingIdentificator));
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            return course;
        }

        public ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators)
        {
            ICollection<Course> Courses = new List<Course>();

            foreach (var courseIdentificator in coursesIdentificators)
            {
                var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
                Course singleCourse = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
                Courses.Add(singleCourse);
            }
            return Courses;
        }

        public void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator)
        {
            Course course = GetCourseById(courseIdentificator);
            course.Meetings.Add(meetingIdentificator);

            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);

            _context.db.GetCollection<Course>(_coursesCollectionName).ReplaceOne(filter, course);
        }
    }
}
