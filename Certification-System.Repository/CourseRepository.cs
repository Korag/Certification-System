﻿using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly MongoContext _context;

        private readonly string _coursesCollectionName = "Courses";
        private readonly string _coursesQueueCollectionName = "CoursesQueue";

        private IMongoCollection<Course> _courses;
        private IMongoCollection<CourseQueue> _coursesQueue;

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

        public ICollection<Course> GetListOfNotStartedCourses()
        {
            return GetCourses().AsQueryable().Where(z => z.DateOfStart > DateTime.Now).ToList();
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

        public ICollection<Course> GetCoursesAfterEndDate()
        {
            var filter = Builders<Course>.Filter.Where(x => x.CourseEnded == false && x.DateOfEnd < DateTime.Now);
            var resultListOfCourses = GetCourses().Find<Course>(filter).ToList();

            return resultListOfCourses;
        }

        public ICollection<SelectListItem> GetActiveCoursesAsSelectList()
        {
            List<Course> courses = GetActiveCourses().ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var course in courses)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " | " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetActiveCoursesWhereExamIsRequiredAsSelectList()
        {
            List<Course> courses = GetActiveCourses().ToList().Where(z => z.ExamIsRequired == true).ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var course in courses)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " | " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetActiveCoursesWithVacantSeatsAsSelectList()
        {
            List<Course> courses = GetActiveCourses().ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var course in courses)
            {
                var courseQueue = GetCourseQueueById(course.CourseIdentificator);

                var vacantSeats = course.EnrolledUsersLimit - course.EnrolledUsers.Count();

                if (courseQueue != null)
                {
                    vacantSeats = vacantSeats - courseQueue.AwaitingUsers.Count();
                }

                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " | " + course.Name + " |wm.: " + vacantSeats,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetAllCoursesAsSelectList()
        {
            GetCourses();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var course in _courses.AsQueryable().ToList())
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " | " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GenerateSelectList(ICollection<string> coursesIdentificators)
        {
            var courses = GetCoursesById(coursesIdentificators);

            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var course in courses)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " | " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return selectList;
        }

        public Course AddEnrolledUserToCourse(string courseIdentificator, string userIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var update = Builders<Course>.Update.AddToSet(x => x.EnrolledUsers, userIdentificator);

            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();
            resultCourse.EnrolledUsers.Add(userIdentificator);

            var result = _courses.UpdateOne(filter, update);

            return resultCourse;
        }

        public Course AddEnrolledUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Course>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var update = Builders<Course>.Update.AddToSetEach(x => x.EnrolledUsers, usersIdentificators);

            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();
            resultCourse.EnrolledUsers.ToList().AddRange(usersIdentificators);

            var result = _courses.UpdateOne(filter, update);

            return resultCourse;
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
                var course = _courses.AsQueryable().ToList().Where(z => z.Exams.Contains(exam.ExamIdentificator)).FirstOrDefault();

                if (resultListOfCourses.Contains(course))
                {
                    resultListOfCourses.Add(course);
                }
            }

            return resultListOfCourses;
        }

        public Course GetCourseByExamId(string examIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Exams.Contains(examIdentificator));
            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();

            return resultCourse;
        }

        public ICollection<Course> GetCoursesByExamsId(ICollection<string> examsIdentificators)
        {
            List<Course> resultListOfCourses = new List<Course>();

            foreach (var examIdentificator in examsIdentificators)
            {
                var filter = Builders<Course>.Filter.Where(x => x.Exams.Contains(examIdentificator));
                resultListOfCourses.Add(GetCourses().Find<Course>(filter).FirstOrDefault());
            }

            return resultListOfCourses;
        }

        public ICollection<Course> DeleteBranchFromCourses(string branchIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(z => z.Branches.Contains(branchIdentificator));
            var update = Builders<Course>.Update.Pull(x => x.Branches, branchIdentificator);

            var resultListOfCourses = GetCourses().Find<Course>(filter).ToList();
            resultListOfCourses.ForEach(z => z.Branches.Remove(branchIdentificator));

            _courses.UpdateMany(filter, update);

            return resultListOfCourses;
        }

        public Course DeleteExamFromCourse(string examIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(z => z.Exams.Contains(examIdentificator));
            var update = Builders<Course>.Update.Pull(x => x.Exams, examIdentificator);

            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();
            resultCourse.Exams.Remove(examIdentificator);

            _courses.UpdateOne(filter, update);

            return resultCourse;
        }

        public Course DeleteMeetingFromCourse(string meetingIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(z => z.Meetings.Contains(meetingIdentificator));
            var update = Builders<Course>.Update.Pull(x => x.Meetings, meetingIdentificator);

            var resultCourse = GetCourses().Find<Course>(filter).FirstOrDefault();
            resultCourse.Meetings.Remove(meetingIdentificator);

            _courses.UpdateOne(filter, update);

            return resultCourse;
        }

        public void DeleteCourse(string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var result = GetCourses().DeleteOne(filter);
        }

        public ICollection<Course> DeleteUserFromCourses(ICollection<string> usersIdentificators)
        {
            var filter = Builders<Course>.Filter.Where(z => z.EnrolledUsers.ToList().Where(x => usersIdentificators.Contains(x)).ToList().Count() != 0);
            var update = Builders<Course>.Update.PullAll(x => x.EnrolledUsers, usersIdentificators);

            var resultListOfCourses = GetCourses().Find<Course>(filter).ToList();

            // to check
            resultListOfCourses.ForEach(z => z.EnrolledUsers.ToList().RemoveAll(x => usersIdentificators.Contains(x)));

            var result = _courses.DeleteMany(filter);

            return resultListOfCourses;
        }

        public string CountCoursesWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetCourses().AsQueryable().Where(z => z.CourseIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }

        #region CourseQueue
        private IMongoCollection<CourseQueue> GetCoursesQueue()
        {
            return _coursesQueue = _context.db.GetCollection<CourseQueue>(_coursesQueueCollectionName);
        }

        public ICollection<CourseQueue> GetListOfCoursesQueue()
        {
            return GetCoursesQueue().AsQueryable().ToList();
        }

        public CourseQueue GetCourseQueueById(string courseIdentificator)
        {
            var filter = Builders<CourseQueue>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            var resultCourseQueue = GetCoursesQueue().Find<CourseQueue>(filter).FirstOrDefault();

            return resultCourseQueue;
        }

        public ICollection<CourseQueue> GetCoursesQueueById(ICollection<string> coursesIdentificators)
        {
            var filter = Builders<CourseQueue>.Filter.Where(z => coursesIdentificators.Contains(z.CourseIdentificator));
            var resultListOfCoursesQueue = GetCoursesQueue().Find<CourseQueue>(filter).ToList();

            return resultListOfCoursesQueue;
        }

        public CourseQueue CreateCourseQueue(string courseIdentificator)
        {
            CourseQueue courseQueue = new CourseQueue
            {
                CourseIdentificator = courseIdentificator
            };

            GetCoursesQueue().InsertOne(courseQueue);

            return courseQueue;
        }

        public void DeleteCourseQueue(string courseIdentificator)
        {
            var filter = Builders<CourseQueue>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var result = GetCoursesQueue().DeleteOne(filter);
        }

        public CourseQueue AddAwaitingUserToCourseQueue(string courseIdentificator, string userIdentificator, LogInformation logData)
        {
            CourseQueueUser user = new CourseQueueUser
            {
                UserIdentificator = userIdentificator,
                LogData = logData
            };

            var filter = Builders<CourseQueue>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
            var update = Builders<CourseQueue>.Update.AddToSet(x => x.AwaitingUsers, user);

            var resultCourseQueue = GetCoursesQueue().Find<CourseQueue>(filter).FirstOrDefault();
            resultCourseQueue.AwaitingUsers.Add(new CourseQueueUser { UserIdentificator = userIdentificator });

            var result = _coursesQueue.UpdateOne(filter, update);

            return resultCourseQueue;
        }

        public CourseQueue AddAwaitingUsersToCourseQueue(string courseIdentificator, ICollection<string> usersIdentificators, LogInformation logData)
        {
            GetCoursesQueue();
            CourseQueue resultCourseQueue = new CourseQueue();

            foreach (var userIdentificator in usersIdentificators)
            {
                CourseQueueUser user = new CourseQueueUser
                {
                    UserIdentificator = userIdentificator,
                    LogData = logData
                };

                var filter = Builders<CourseQueue>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);
                var update = Builders<CourseQueue>.Update.AddToSet(x => x.AwaitingUsers, user);

                resultCourseQueue = _coursesQueue.Find<CourseQueue>(filter).FirstOrDefault();
                resultCourseQueue.AwaitingUsers.Add(new CourseQueueUser { UserIdentificator = userIdentificator });

                var result = _coursesQueue.UpdateOne(filter, update);
            }

            return resultCourseQueue;
        }

        public CourseQueue RemoveAwaitingUserFromCourseQueue(string courseIdentificator, string userIdentificator)
        {
            var filter = Builders<CourseQueue>.Filter.Where(z => z.CourseIdentificator == courseIdentificator);

            var resultCourseQueue = GetCoursesQueue().Find<CourseQueue>(filter).FirstOrDefault();
            var courseQueueUser = resultCourseQueue.AwaitingUsers.Where(z => z.UserIdentificator == userIdentificator).FirstOrDefault();

            resultCourseQueue.AwaitingUsers.Remove(courseQueueUser);

            _coursesQueue.ReplaceOne(filter, resultCourseQueue);

            return resultCourseQueue;
        }
        #endregion
    }
}
