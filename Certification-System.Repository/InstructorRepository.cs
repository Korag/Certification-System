using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly MongoContext _context;

        private readonly string _instructorsCollectionName = "Instructors";
        private IMongoCollection<Instructor> _instructors;

        public InstructorRepository(MongoContext context)
        {
            _context = context;
        }

        public ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId)
        {
            List<Instructor> Instructors = new List<Instructor>();

            foreach (var instructor in InstructorsId)
            {
                var filter = Builders<Instructor>.Filter.Eq(x => x.InstructorIdentificator, instructor);
                Instructor singleInstructor = _context.db.GetCollection<Instructor>(_instructorsCollectionName).Find<Instructor>(filter).FirstOrDefault();
                Instructors.Add(singleInstructor);
            }

            return Instructors;
        }

        public Instructor GetInstructorById(string instructorIdentificator)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.InstructorIdentificator, instructorIdentificator);
            Instructor instructor = _context.db.GetCollection<Instructor>(_instructorsCollectionName).Find<Instructor>(filter).FirstOrDefault();

            return instructor;
        }

        public void AddInstructor(Instructor instructor)
        {
            _instructors = _context.db.GetCollection<Instructor>(_instructorsCollectionName);
            _instructors.InsertOne(instructor);
        }

        public ICollection<Instructor> GetInstructors()
        {
            _instructors = _context.db.GetCollection<Instructor>(_instructorsCollectionName);

            return _instructors.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetInstructorsAsSelectList()
        {
            List<Instructor> Instructors = GetInstructors().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var instructor in Instructors)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = instructor.FirstName + " " + instructor.LastName,
                            Value = instructor.InstructorIdentificator
                        }
                    );
            };

            return SelectList;
        }
    }
}
