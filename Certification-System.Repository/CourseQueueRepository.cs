using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class CourseQueueRepository : ICourseQueueRepository
    {
        private readonly MongoContext _context;

        private readonly string _coursesCollectionName = "CoursesQueue";
        private IMongoCollection<CourseQueue> _coursesQueue;

        public CourseQueueRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<CourseQueue> GetCoursesQueue()
        {
            return _coursesQueue = _context.db.GetCollection<CourseQueue>(_coursesCollectionName);
        }

        public ICollection<CourseQueue> GetListOfCoursesQueue()
        {
            return GetCoursesQueue().AsQueryable().ToList();
        }

        
    }
}
