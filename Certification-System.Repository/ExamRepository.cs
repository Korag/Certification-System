using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;

namespace Certification_System.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly MongoContext _context;

        private readonly string _examsCollectionName = "Exams";
        private readonly string _examsTermsCollectionName = "ExamsTerms";
        private readonly string _examsResultsCollectionName = "ExamsResults";

        private IMongoCollection<Exam> _exams;
        private IMongoCollection<ExamTerm> _examsTerms;
        private IMongoCollection<ExamResult> _examsResults;

        public ExamRepository(MongoContext context)
        {
            _context = context;
        }
    }
}
