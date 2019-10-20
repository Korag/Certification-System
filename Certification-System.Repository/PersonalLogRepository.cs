using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Certification_System.ServicesInterfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class PersonalLogRepository : IPersonalLogRepository
    {
        private readonly MongoContext _context;
        private IUserRepository _userRepository;

        private readonly string _personalLogCollectionName = "PersonalLogs";
        private IMongoCollection<PersonalLog> _personalLogs;

        private readonly string _rejectedUsersLogCollectionName = "RejectedUsersLogs";
        private IMongoCollection<RejectedUserFromCourseQueueLog> _rejectedUsersLogs;

        private IIPGetterService _ipGetter;
        private IKeyGenerator _keyGenerator;

        public PersonalLogRepository(
                     MongoContext context,
                     IUserRepository userRepository,
                     IIPGetterService ipGetter,
                     IKeyGenerator keyGenerator)
        {
            _context = context;
            _userRepository = userRepository;
            _ipGetter = ipGetter;
            _keyGenerator = keyGenerator;
        }

        public PersonalLogInformation GeneratePersonalLogInformation(
                                              string userEmailAddress,
                                              string actionName,
                                              string typeOfAction,
                                              string urlToActionDetails,
                                              string descriptionOfAction)
        {
            var user = _userRepository.GetUserByEmail(userEmailAddress);

            return new PersonalLogInformation
            {
                ChangeAuthorEmail = user.Email,
                ChangeAuthorFirstName = user.FirstName,
                ChangeAuthorLastName = user.LastName,
                ChangeAuthorIdentificator = user.Id,

                DateOfLogCreation = DateTime.Now,
                IpAddress = _ipGetter.GetGlobalIPAddress(),

                TypeOfAction = typeOfAction,
                DescriptionOfAction = descriptionOfAction,

                ActionName = actionName,
                UrlToDetailsOfAction = urlToActionDetails,
            };
        }

        private void AddLogToPersonalUserLogs(string userIdentificator, PersonalLogInformation logInfo)
        {
            var filter = Builders<PersonalLog>.Filter.Where(z => z.UserIdentificator == userIdentificator);
            var update = Builders<PersonalLog>.Update.AddToSet(x => x.LogData, logInfo);

            var result = _personalLogs.UpdateOne(filter, update);
        }

        private void AddRejectedUserLog(RejectedUserFromCourseQueueLog rejectedUser)
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

        public ICollection<RejectedUserFromCourseQueueLog> GetListOfRejectedUsers()
        {
            return GetRejectedUsersLogs().AsQueryable().ToList();
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
            };

            GetPersonalLogs().InsertOne(personalLog);
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

        // todo later action, View, dataTable
        public void AddRejectedUserFromCourseQueueLog(CourseQueueWithSingleUser rejectedUser, LogInformation logInfo)
        {
            var rejectedUserLog = new RejectedUserFromCourseQueueLog
            {
                RejectedUserFromCourseQueueLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = rejectedUser,
                LogData = logInfo
            };

            AddRejectedUserLog(rejectedUserLog);
        }

        public void AddRejectedUsersFromCourseQueueLog(ICollection<CourseQueueWithSingleUser> rejectedUsers, LogInformation logInfo)
        {
            foreach (var rejectedUser in rejectedUsers)
            {
                var rejectedUserLog = new RejectedUserFromCourseQueueLog
                {
                    RejectedUserFromCourseQueueLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = rejectedUser,
                    LogData = logInfo
                };

                AddRejectedUserLog(rejectedUserLog);
            }
        }
    }
}
