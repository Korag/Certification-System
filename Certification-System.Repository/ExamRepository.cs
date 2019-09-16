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

        public ICollection<Exam> GetListOfActiveExams()
        {
            return GetExams().AsQueryable().Where(z => (z.DateOfStart > DateTime.Now) && (z.ExamDividedToTerms == false) || (z.DateOfEnd > DateTime.Now) && (z.ExamDividedToTerms == true)).ToList();
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

        public ICollection<Exam> GetExamsByExaminerId(string userIdentificator)
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
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public IList<SelectListItem> GetActiveExamsAsSelectList()
        {
            var Exams = GetListOfActiveExams();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var exam in Exams)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetExamsWhichAreDividedToTermsAsSelectList()
        {
            var Exams = GetListOfExams().ToList().Where(z => z.ExamDividedToTerms == true);
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var exam in Exams)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public Exam GetExamByExamTermId(string examTermIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamTerms.Contains(examTermIdentificator));
            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();

            return resultExam;
        }

        public ICollection<SelectListItem> GetAddExamMenuOptions()
        {
            var Exams = GetListOfExams();
            List<SelectListItem> SelectList = new List<SelectListItem>
                {
                new SelectListItem(){
                            Text = "Dodaj egzamin",
                            Value = "addExam"
                },
                new SelectListItem(){
                            Text = "Dodaj termin do istniejącego egzaminu",
                            Value = "addExamPeriod"
                },
            };

            return SelectList;
        }

        public IList<SelectListItem> GetActiveExamsWithVacantSeatsAsSelectList()
        {
            List<Exam> Exams = GetListOfActiveExams().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var exam in Exams)
            {
                var VacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name + " |wm.: " + VacantSeats,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public Exam GetExamByExamResultId(string examResultId)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamResults.Contains(examResultId));
            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();

            return resultExam;
        }

        public void AddUserToExam(string examIdentificator, string userIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var update = Builders<Exam>.Update.AddToSet(x => x.EnrolledUsers, userIdentificator);

            var result = GetExams().UpdateOne(filter, update);
        }

        public void DeleteUsersFromExam(string examIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var update = Builders<Exam>.Update.PullAll(x => x.EnrolledUsers, usersIdentificators);

            var result = GetExams().UpdateOne(filter, update);
        }

        public void AddUsersToExam(string examIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var update = Builders<Exam>.Update.AddToSetEach(x => x.EnrolledUsers, usersIdentificators);

            var result = GetExams().UpdateOne(filter, update);
        }

        public void SetMaxAmountOfPointsToEarn(string examIdentificator, double maxAmountOfPointsToEarn)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var update = Builders<Exam>.Update.Set(x => x.MaxAmountOfPointsToEarn, maxAmountOfPointsToEarn);

            var result = GetExams().UpdateOne(filter, update);
        }

        public ICollection<Exam> GetExamPeriods(string examIndexer)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIndexer == examIndexer);
            var resultListOfExams = GetExams().Find<Exam>(filter).ToList();

            return resultListOfExams;
        }

        public Exam DeleteExamResultFromExam(string examResultIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamResults.Contains(examResultIdentificator));
            var update = Builders<Exam>.Update.Pull(x => x.ExamResults, examResultIdentificator);

            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();
            resultExam.ExamResults.Remove(examResultIdentificator);

            var result = _exams.UpdateOne(filter, update);

            return resultExam;
        }

        public Exam DeleteExamTermFromExam(string examTermIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamTerms.Contains(examTermIdentificator));
            var update = Builders<Exam>.Update.Pull(x => x.ExamTerms, examTermIdentificator);

            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();
            resultExam.ExamTerms.Remove(examTermIdentificator);

            var result = _exams.UpdateOne(filter, update);

            return resultExam;
        }
    }
}
