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

        public ExamTerm GetExamTermById(string examTermIdentificator)
        {
            var filter = Builders<ExamTerm>.Filter.Eq(x => x.ExamTermIdentificator, examTermIdentificator);
            var resultExamTerm = GetExamsTerms().Find<ExamTerm>(filter).FirstOrDefault();

            return resultExamTerm;
        }

        public ICollection<ExamTerm> GetExamsTermsById(ICollection<string> examsTermsIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => examsTermsIdentificators.Contains(z.ExamTermIdentificator));
            var resultListOfExamTerms = GetExamsTerms().Find<ExamTerm>(filter).ToList();

            return resultListOfExamTerms;
        }

        public ICollection<ExamTerm> GetExamTermsByExaminerId(string userIdentificator)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => z.Examiners.Contains(userIdentificator));
            var resultListOfExamTerms = GetExamsTerms().Find<ExamTerm>(filter).ToList();

            return resultListOfExamTerms;
        }

        public void UpdateExamsTerms(ICollection<ExamTerm> examTerms)
        {
            foreach (var examTerm in examTerms)
            {
            var filter = Builders<ExamTerm>.Filter.Eq(x => x.ExamTermIdentificator, examTerm.ExamTermIdentificator);
            var result = GetExamsTerms().ReplaceOne(filter, examTerm);
            }
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            var filter = Builders<ExamTerm>.Filter.Eq(x => x.ExamTermIdentificator, examTerm.ExamTermIdentificator);
            var result = GetExamsTerms().ReplaceOne(filter, examTerm);
        }

        public void DeleteExamsTerms(ICollection<string> examsTermsIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(x => examsTermsIdentificators.Contains(x.ExamTermIdentificator));
            var result = GetExamsTerms().DeleteMany(filter);
        }
    }
}
