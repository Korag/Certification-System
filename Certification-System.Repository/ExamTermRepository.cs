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
        public ICollection<ExamTerm> GetListOfActiveExamsTerms()
        {
            return GetExamsTerms().AsQueryable().Where(z=> z.DateOfStart > DateTime.Now).ToList();
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

        public ICollection<ExamTerm> GetActiveExamsTermsById(ICollection<string> examsTermsIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => examsTermsIdentificators.Contains(z.ExamTermIdentificator) && z.DateOfStart > DateTime.Now);
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

        public ICollection<ExamTerm> DeleteExamsTerms(ICollection<string> examsTermsIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(x => examsTermsIdentificators.Contains(x.ExamTermIdentificator));
            var resultListOfExamsTerms = GetExamsTerms().Find<ExamTerm>(filter).ToList();

            var result = GetExamsTerms().DeleteMany(filter);

            return resultListOfExamsTerms;
        }

        public IList<SelectListItem> GetActiveExamTermsWithVacantSeatsAsSelectList(Exam exam)
        {
            var ExamTerms = GetActiveExamsTermsById(exam.ExamTerms).ToList();

            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var examTerm in ExamTerms)
            {
                var VacantSeats = examTerm.UsersLimit - examTerm.EnrolledUsers.Count();

                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = exam.DateOfStart + " - " + exam.DateOfEnd + " |wm.: " + VacantSeats,
                            Value = exam.ExamIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public IList<SelectListItem> GetActiveExamTermsWithVacantSeatsAsSelectList()
        {
            var ExamTerms = GetListOfActiveExamsTerms().ToList();

            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var examTerm in ExamTerms)
            {
                var VacantSeats = examTerm.UsersLimit - examTerm.EnrolledUsers.Count();

                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = examTerm.DateOfStart + " - " + examTerm.DateOfEnd + " |wm.: " + VacantSeats,
                            Value = examTerm.ExamTermIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public void AddUserToExamTerm(string examTermIdentificator, string userIdentificator)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => z.ExamTermIdentificator == examTermIdentificator);
            var update = Builders<ExamTerm>.Update.AddToSet(x => x.EnrolledUsers, userIdentificator);

            var result = GetExamsTerms().UpdateOne(filter, update);
        }

        public void DeleteUsersFromExamTerms(ICollection<string> examTermsIdentificators, ICollection<string> usersIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => examTermsIdentificators.Contains(z.ExamTermIdentificator));
            var update = Builders<ExamTerm>.Update.PullAll(x => x.EnrolledUsers, usersIdentificators);

            var result = GetExamsTerms().UpdateOne(filter, update);
        }

        public void DeleteUsersFromExamTerm(string examTermIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => z.ExamTermIdentificator == examTermIdentificator);
            var update = Builders<ExamTerm>.Update.PullAll(x => x.EnrolledUsers, usersIdentificators);

            var result = GetExamsTerms().UpdateOne(filter, update);
        }

        public void AddUsersToExamTerm(string examTermIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => z.ExamTermIdentificator == examTermIdentificator);
            var update = Builders<ExamTerm>.Update.AddToSetEach(x => x.EnrolledUsers, usersIdentificators);

            var result = GetExamsTerms().UpdateOne(filter, update);
        }

        public void DeleteExamTerm(string examTermIdentificator)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => z.ExamTermIdentificator == examTermIdentificator);
            var result = GetExamsTerms().DeleteOne(filter);
        }

        public ICollection<ExamTerm> DeleteUserFromExamsTerms(string userIdentificator, ICollection<string> examsTermsIdentificators)
        {
            var filter = Builders<ExamTerm>.Filter.Where(z => examsTermsIdentificators.Contains(z.ExamTermIdentificator) && (z.EnrolledUsers.Contains(userIdentificator) || z.Examiners.Contains(userIdentificator)));
            var update = Builders<ExamTerm>.Update.Pull(x => x.EnrolledUsers, userIdentificator).Pull(x => x.Examiners, userIdentificator);

            List<ExamTerm> resultListOfExamsTerms = new List<ExamTerm>();

            if (examsTermsIdentificators.Count() != 0)
            {
                var examsTerms = (IMongoCollection<ExamTerm>)examsTermsIdentificators;
                resultListOfExamsTerms = examsTerms.Find<ExamTerm>(filter).ToList();
            }
            else
            {
                resultListOfExamsTerms = GetExamsTerms().Find<ExamTerm>(filter).ToList();
            }

            resultListOfExamsTerms.ForEach(z => z.EnrolledUsers.Remove(userIdentificator));
            resultListOfExamsTerms.ForEach(z => z.Examiners.Remove(userIdentificator));

            var result = _examsTerms.UpdateMany(filter, update);

            return resultListOfExamsTerms;
        }

        public string CountExamsTermsWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetExamsTerms().AsQueryable().Where(z => z.ExamTermIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }
    }
}
