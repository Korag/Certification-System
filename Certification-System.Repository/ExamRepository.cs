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

        private ICollection<Exam> GetActiveExams()
        {
            return GetExams().AsQueryable().Where(z => (z.DateOfStart > DateTime.Now) && (z.ExamDividedToTerms == false) || (z.DateOfEnd > DateTime.Now) && (z.ExamDividedToTerms == true)).ToList();
        }

        public ICollection<Exam> GetListOfExams()
        {
            return GetExams().AsQueryable().ToList();
        }

        public ICollection<Exam> GetListOfFirstPeriodExams()
        {
            return GetExams().AsQueryable().Where(z => z.OrdinalNumber == 1).ToList();
        }
        
        public ICollection<Exam> GetListOfActiveExams()
        {
            return GetActiveExams().AsQueryable().ToList();
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

        public ICollection<Exam> GetOnlyActiveExamsDividedToTermsById(ICollection<string> examsIdentificators)
        {
            return GetActiveExams().Where(z => examsIdentificators.Contains(z.ExamIdentificator) && z.ExamDividedToTerms).ToList();
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
            var exams = GetListOfExams();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
        }

        public IList<SelectListItem> GetFirstPeriodExamsAsSelectList()
        {
            var exams = GetListOfFirstPeriodExams();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
        }

        public IList<SelectListItem> GetActiveExamsAsSelectList()
        {
            var exams = GetListOfActiveExams();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetExamsWhichAreDividedToTermsAsSelectList()
        {
            var exams = GetListOfExams().ToList().Where(z => z.ExamDividedToTerms == true);
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
        }

        public Exam GetExamByExamTermId(string examTermIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamTerms.Contains(examTermIdentificator));
            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();

            return resultExam;
        }

        public Exam GetExamByIndexer(string examIndexer)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIndexer == examIndexer);
            var resultListofExams = GetExams().Find<Exam>(filter).ToList();

            return resultListofExams.OrderBy(z=> z.OrdinalNumber).FirstOrDefault();
        }

        public ICollection<SelectListItem> GetAddExamMenuOptions()
        {
            List<SelectListItem> selectList = new List<SelectListItem>
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

            return selectList;
        }

        public IList<SelectListItem> GetActiveExamsWithVacantSeatsAsSelectList()
        {
            List<Exam> exams = GetListOfActiveExams().ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                var vacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " |Term." + exam.OrdinalNumber + " | " + exam.Name + " |wm.: " + vacantSeats,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
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

        public void AddExamsResultsToExam(string examIdentificator, ICollection<string> examsResultsIdentificators)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var update = Builders<Exam>.Update.AddToSetEach(x => x.ExamResults, examsResultsIdentificators);

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

        public void DeleteExam(string examIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator);
            var result = GetExams().DeleteOne(filter);
        }

        public ICollection<Exam> DeleteExams(ICollection<string> examsIdentificators)
        {
            var filter = Builders<Exam>.Filter.Where(z => examsIdentificators.Contains(z.ExamIdentificator));

            var resultListOfExams = GetExams().Find<Exam>(filter).ToList();
            var result = _exams.DeleteMany(filter);

            return resultListOfExams;
        }

        public Exam DeleteUserFromExam(string userIdentificator, string examIdentificator)
        {
            var filter = Builders<Exam>.Filter.Where(z => z.ExamIdentificator == examIdentificator && z.EnrolledUsers.Contains(userIdentificator));
            var update = Builders<Exam>.Update.Pull(x => x.EnrolledUsers, userIdentificator);

            var resultExam = GetExams().Find<Exam>(filter).FirstOrDefault();
            resultExam.EnrolledUsers.Remove(userIdentificator);

            var result = _exams.UpdateOne(filter, update);

            return resultExam;
        }

        public ICollection<Exam> DeleteUserFromExams(string userIdentificator, ICollection<string> examsIdentificators = null)
        {
            var filter = Builders<Exam>.Filter.Where(z => examsIdentificators.Contains(z.ExamIdentificator) && (z.EnrolledUsers.Contains(userIdentificator) || z.Examiners.Contains(userIdentificator)));
            var update = Builders<Exam>.Update.Pull(x => x.EnrolledUsers, userIdentificator).Pull(x => x.Examiners, userIdentificator);

            List<Exam> resultListOfExams = new List<Exam>();

            if (examsIdentificators.Count() != 0)
            {
                var examsTerms = (IMongoCollection<Exam>)examsIdentificators;
                resultListOfExams = examsTerms.Find<Exam>(filter).ToList();
            }
            else
            {
                resultListOfExams = GetExams().Find<Exam>(filter).ToList();
            }

            resultListOfExams.ForEach(z => z.EnrolledUsers.Remove(userIdentificator));
            resultListOfExams.ForEach(z => z.Examiners.Remove(userIdentificator));

            var result = _exams.UpdateMany(filter, update);

            return resultListOfExams;
        }

        public string CountExamsWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetExams().AsQueryable().Where(z => z.ExamIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }

        public IList<SelectListItem> GetExamsByIdAsSelectList(ICollection<string> examsIdentificators)
        {
            List<Exam> exams = GetExamsById(examsIdentificators).ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var exam in exams)
            {
                var VacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.ExamIndexer + " | " + exam.Name,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return selectList;
        }

        public IList<SelectListItem> GetExamsTypesAsSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var examType in ExamsTypes.TypesOfExams)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = examType.Value,
                            Value = examType.Value
                        }
                    );
            };

            return selectList;
        }
    }
}
