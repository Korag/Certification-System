using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Certification_System.ServicesInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class PersonalLogRepository : IPersonalLogRepository
    {
        private readonly MongoContext _context;
        private IUserRepository _userRepository;

        private readonly string _personalLogCollectionName = "PersonalLogs";
        private IMongoCollection<PersonalLog> _personalLogs;

        private readonly string _adminPersonalLogCollectionName = "AdminPersonalLogs";
        private IMongoCollection<PersonalLog> _adminPersonalLogs;

        private readonly string _rejectedUsersLogCollectionName = "RejectedUsersLogs";
        private IMongoCollection<RejectedUserFromCourseQueueLog> _rejectedUsersLogs;

        private IIPGetterService _ipGetter;

        public PersonalLogRepository(
                     MongoContext context,
                     IUserRepository userRepository,
                     IIPGetterService ipGetter)
        {
            _context = context;
            _userRepository = userRepository;
            _ipGetter = ipGetter;
        }

        public PersonalLogInformation GeneratePersonalLogInformation(
                                              string userEmail,
                                              string actionName,
                                              string descriptionOfAction,
                                              string additionalInfo = "")
        {
            var user = _userRepository.GetUserByEmail(userEmail);

            return new PersonalLogInformation
            {
                ChangeAuthorEmail = user.Email,
                ChangeAuthorFirstName = user.FirstName,
                ChangeAuthorLastName = user.LastName,
                ChangeAuthorIdentificator = user.Id,

                DateOfLogCreation = DateTime.Now,
                IpAddress = _ipGetter.GetGlobalIPAddress(),

                DescriptionOfAction = descriptionOfAction,
                AdditionalInfo = additionalInfo,

                ActionName = actionName,
            };
        }

        private void AddLogToPersonalUserLogs(string userIdentificator, PersonalLogInformation logInfo)
        {
            var filter = Builders<PersonalLog>.Filter.Where(z => z.UserIdentificator == userIdentificator);
            var update = Builders<PersonalLog>.Update.AddToSet(x => x.LogData, logInfo);

            var result = GetPersonalLogs().UpdateOne(filter, update);
        }

        private void AddLogToPersonalAdminUserLogs(string userIdentificator, PersonalLogInformation logInfo)
        {
            var filter = Builders<PersonalLog>.Filter.Where(z => z.UserIdentificator == userIdentificator);
            var update = Builders<PersonalLog>.Update.AddToSet(x => x.LogData, logInfo);

            var result = GetAdminPersonalLogs().UpdateOne(filter, update);
        }

        public void AddRejectedUserLog(RejectedUserFromCourseQueueLog rejectedUser)
        {
            GetRejectedUsersLogs().InsertOne(rejectedUser);
        }

        private IMongoCollection<RejectedUserFromCourseQueueLog> GetRejectedUsersLogs()
        {
            return _rejectedUsersLogs = _context.db.GetCollection<RejectedUserFromCourseQueueLog>(_rejectedUsersLogCollectionName);
        }

        private IMongoCollection<PersonalLog> GetPersonalLogs()
        {
            return _personalLogs = _context.db.GetCollection<PersonalLog>(_personalLogCollectionName);
        }

        private IMongoCollection<PersonalLog> GetAdminPersonalLogs()
        {
            return _adminPersonalLogs = _context.db.GetCollection<PersonalLog>(_adminPersonalLogCollectionName);
        }

        public ICollection<RejectedUserFromCourseQueueLog> GetListOfRejectedUsers()
        {
            return GetRejectedUsersLogs().AsQueryable().ToList();
        }

        public ICollection<PersonalLog> GetListOfPersonalLogs()
        {
            return GetPersonalLogs().AsQueryable().ToList();
        }

        public PersonalLog GetListOfAdminPersonalLogs()
        {
            return GetAdminPersonalLogs().AsQueryable().FirstOrDefault();
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
            };

            GetPersonalLogs().InsertOne(personalLog);
        }

        private void CreateAdminPersonalUserLog()
        {
            PersonalLog personalLog = new PersonalLog
            {
                UserIdentificator = ObjectId.GenerateNewId().ToString()
            };

            GetAdminPersonalLogs().InsertOne(personalLog);
        }

        public void AddPersonalUserLog(string userIdentificator, PersonalLogInformation logInfo)
        {
            var userLog = GetPersonalUserLogById(userIdentificator);
            var user = _userRepository.GetUserById(userIdentificator);

            if (userLog == null)
            {
                CreatePersonalUserLog(user);
            }

            AddLogToPersonalUserLogs(userIdentificator, logInfo);
        }

        public void AddPersonalUsersLogs(ICollection<string> usersIdentificators, PersonalLogInformation logInfo)
        {
            foreach (var userIdentificator in usersIdentificators)
            {
                var user = _userRepository.GetUserById(userIdentificator);
                var userLog = GetPersonalUserLogById(userIdentificator);

                if (userLog == null)
                {
                    CreatePersonalUserLog(user);
                }

                AddLogToPersonalUserLogs(userIdentificator, logInfo);
            }
        }

        public void AddPersonalUsersMultipleLogs(ICollection<string> usersIdentificators, ICollection<PersonalLogInformation> logInfoCollection)
        {
            foreach (var userIdentificator in usersIdentificators)
            {
                var user = _userRepository.GetUserById(userIdentificator);
                var userLog = GetPersonalUserLogById(userIdentificator);

                if (userLog == null)
                {
                    CreatePersonalUserLog(user);
                }

                foreach (var logInfo in logInfoCollection)
                {
                    AddLogToPersonalUserLogs(userIdentificator, logInfo);
                }
            }
        }

        public void AddPersonalUserLogToAdminGroup(PersonalLogInformation logInfo)
        {
            var adminLog = GetListOfAdminPersonalLogs();

            if (adminLog == null)
            {
                CreateAdminPersonalUserLog();
            }

            AddLogToPersonalAdminUserLogs(adminLog.UserIdentificator, logInfo);
        }

        public void AddPersonalUsersLogsToAdminGroup(ICollection<PersonalLogInformation> logInfoCollection)
        {
            var adminLog = GetListOfAdminPersonalLogs();

            if (adminLog == null)
            {
                CreateAdminPersonalUserLog();
            }

            foreach (var logInfo in logInfoCollection)
            {
                AddLogToPersonalAdminUserLogs(adminLog.UserIdentificator, logInfo);
            }
        }
    }
}

