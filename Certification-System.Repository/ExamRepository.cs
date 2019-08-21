using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly MongoContext _context;

        private readonly string _examsCollectionName = "Exams";
        private IMongoCollection<Exam> _exams;

        public ExamRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<Exam> GetExams()
        {
            return _exams = _context.db.GetCollection<Exam>(_examsCollectionName);
        }

        public ICollection<Exam> GetListOfExams()
        {
            return GetExams().AsQueryable().ToList();
        }

        public void AddExam(Exam exam)
        {
            GetExams().InsertOne(exam);
        }
    }
}
