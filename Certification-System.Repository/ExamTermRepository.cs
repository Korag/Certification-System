using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class ExamTermRepository : IExamTermRepository
    {
        private readonly MongoContext _context;

        private readonly string _examsTermsCollectionName = "ExamsTerms";
        private IMongoCollection<ExamTerm> _examsTerms;

        public ExamTermRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<ExamTerm> GetExamsTerms()
        {
            return _examsTerms = _context.db.GetCollection<ExamTerm>(_examsTermsCollectionName);
        }

        public ICollection<ExamTerm> GetListOfExamsTerms()
        {
            return GetExamsTerms().AsQueryable().ToList();
        }

        public void AddExamTerm(ExamTerm examTerm)
        {
            GetExamsTerms().InsertOne(examTerm);
        }

        public void AddExamsTerms(ICollection<ExamTerm> examsTerms)
        {
            GetExamsTerms().InsertMany(examsTerms);
        }
    }
}
