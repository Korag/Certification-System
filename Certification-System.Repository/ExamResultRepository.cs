using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class ExamResultRepository : IExamResultRepository
    {
        private readonly MongoContext _context;

        private readonly string _examsResultsCollectionName = "ExamsResults";
        private IMongoCollection<ExamResult> _examsResults;

        public ExamResultRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<ExamResult> GetExamsResults()
        {
            return _examsResults = _context.db.GetCollection<ExamResult>(_examsResultsCollectionName);
        }

        public ICollection<ExamResult> GetListOfExamsResults()
        {
            return GetExamsResults().AsQueryable().ToList();
        }

        public void AddExamResult(ExamResult examResult)
        {
            GetExamsResults().InsertOne(examResult);
        }

        public void AddExamsResults(ICollection<ExamResult> examsResults)
        {
            GetExamsResults().InsertMany(examsResults);
        }
    }
}
