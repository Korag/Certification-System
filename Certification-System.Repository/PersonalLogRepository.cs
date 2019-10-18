using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class PersonalLogRepository : IPersonalLogRepository
    {
        private readonly MongoContext _context;
    
        private readonly string _personalLogCollectionName = "PersonalLogs";
        private IMongoCollection<PersonalLog> _personalLogs;

        public PersonalLogRepository(MongoContext context)
        {
            _context = context;
        }

        public void AddLogToPersonalUserLogs(string userIdentificator, LogInformation logInfo)
        {
            var filter = Builders<PersonalLog>.Filter.Where(z => z.UserIdentificator == userIdentificator);
            var update = Builders<PersonalLog>.Update.AddToSet(x => x.LogData, logInfo);

            var result = _personalLogs.UpdateOne(filter, update);
        }

        private IMongoCollection<PersonalLog> GetPersonalLogs()
        {
            return _personalLogs = _context.db.GetCollection<PersonalLog>(_personalLogCollectionName);
        }

        public ICollection<PersonalLog> GetListOfPersonalLogs()
        {
            return GetPersonalLogs().AsQueryable().ToList();
        }

        public PersonalLog GetPersonalUserLogById(string userIdentificator)
        {
            var filter = Builders<PersonalLog>.Filter.Eq(x => x.UserIdentificator, userIdentificator);
            var resultPersonalLog = GetPersonalLogs().Find<PersonalLog>(filter).FirstOrDefault();

            return resultPersonalLog;
        }

        public ICollection<PersonalLog> GetPersonalUsersLogsById(ICollection<string> usersIdentificators)
        {
            var filter = Builders<PersonalLog>.Filter.Where(z => usersIdentificators.Contains(z.UserIdentificator));
            var resultListOfPersonalLog = GetPersonalLogs().Find<PersonalLog>(filter).ToList();

            return resultListOfPersonalLog;
        }

        public void CreatePersonalUserLog(CertificationPlatformUser user)
        {
            PersonalLog personalLog = new PersonalLog
            {
                UserIdentificator = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            GetPersonalLogs().InsertOne(personalLog);
        }
    }
}
