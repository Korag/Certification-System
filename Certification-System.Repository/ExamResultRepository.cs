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

        public ExamResult GetExamResultById(string examResultIdentificator)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => z.ExamResultIdentificator == examResultIdentificator);
            var resultExamResult = GetExamsResults().Find<ExamResult>(filter).FirstOrDefault();

            return resultExamResult;
        }

        public ICollection<ExamResult> GetExamsResultsById(ICollection<string> examResultsIdentificators)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => examResultsIdentificators.Contains(z.ExamResultIdentificator));
            var resultListOfExamResults = GetExamsResults().Find<ExamResult>(filter).ToList();

            return resultListOfExamResults;
        }

        public ICollection<ExamResult> GetExamsResultsByExamTermId(string examTermIdentificator)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => z.ExamTerm == examTermIdentificator);
            var resultListOfExamResults = GetExamsResults().Find<ExamResult>(filter).ToList();

            return resultListOfExamResults;
        }

        public void DeleteExamResult(string examResultIdentificator)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => z.ExamResultIdentificator == examResultIdentificator);
            var result = GetExamsResults().DeleteOne(filter);
        }

        public ICollection<ExamResult> DeleteExamsResultsByExamTermId(string examTermIdentificator)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => z.ExamTerm == examTermIdentificator);

            var resultListOfExamsResults = GetExamsResults().Find<ExamResult>(filter).ToList();
            var result = _examsResults.DeleteMany(filter);

            return resultListOfExamsResults;
        }

        public ICollection<ExamResult> DeleteExamsResults(ICollection<string> examsResultsIdentificators)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => examsResultsIdentificators.Contains(z.ExamTerm));

            var resultListOfExamsResults = GetExamsResults().Find<ExamResult>(filter).ToList();
            var result = _examsResults.DeleteMany(filter);

            return resultListOfExamsResults;
        }

        public ICollection<ExamResult> DeleteExamsResultsByUserId(string userIdentificator)
        {
            var filter = Builders<ExamResult>.Filter.Where(z => z.User == userIdentificator);

            var resultListOfExamsResults = GetExamsResults().Find<ExamResult>(filter).ToList();
            var result = _examsResults.DeleteMany(filter);

            return resultListOfExamsResults;
        }
    }
}
