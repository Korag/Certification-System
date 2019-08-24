using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

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

        public Exam GetExamById(string examIdentificator)
        {
            var filter = Builders<Exam>.Filter.Eq(x => x.ExamIdentificator, examIdentificator);
            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();

            return resultExam;
        }

        public ICollection<Exam> GetExamsById(ICollection<string> examsIdentificators)
        {
            var filter = Builders<Exam>.Filter.Where(z => examsIdentificators.Contains(z.ExamIdentificator));
            var resultListOfExams = GetExams().Find<Exam>(filter).ToList();

            return resultListOfExams;
        }

        public ICollection<Exam> GetExamsByExaminatorId(string userIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.Examiners.Contains(userIdentificator));
            var resultListOfExams = GetExams().Find<Exam>(filter).ToList();

            return resultListOfExams;
        }

        public void UpdateExam(Exam exam)
        {
            var filter = Builders<Exam>.Filter.Eq(x => x.ExamIdentificator, exam.ExamIdentificator);
            var result = GetExams().ReplaceOne(filter, exam);
        }

        public ICollection<SelectListItem> GetExamsAsSelectList()
        {
            var Exams = GetListOfExams();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var exam in Exams)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer  + " " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetExamsWhichAreDividedToTermsAsSelectList()
        {
            var Exams = GetListOfExams().ToList().Where(z=> z.ExamDividedToTerms == true);
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var exam in Exams)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }
    }
}
