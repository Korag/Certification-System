using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly MongoContext _context;

        private readonly string _branchesLogCollectionName = "BranchesLogs";
        private readonly string _certificatesLogCollectionName = "CertificatesLogs";
        private readonly string _companiesLogCollectionName = "CompaniesLogs";
        private readonly string _coursesLogCollectionName = "CoursesLogs";
        private readonly string _coursesQueueLogCollectionName = "CoursesQueueLogs";
        private readonly string _degreesLogCollectionName = "DegreesLogs";
        private readonly string _examsLogCollectionName = "ExamsLogs";
        private readonly string _examsResultsLogCollectionName = "ExamsResultsLogs";
        private readonly string _examsTermsLogCollectionName = "ExamsTermsLogs";
        private readonly string _givenCertificatesLogCollectionName = "GivenCertificatesLogs";
        private readonly string _givenDegreesLogCollectionName = "GivenDegreesLogs";
        private readonly string _meetingsLogCollectionName = "MeetingsLogs";
        private readonly string _usersLogCollectionName = "UsersLogs";
        private readonly string _usersLoginLogCollectionName = "UsersLoginLogs";

        private readonly string _personalLogCollectionName = "PersonalLogs";
        private IMongoCollection<PersonalLog> _personalLogs;

        public LogRepository(MongoContext context)
        {
            _context = context;
        }

        public void AddBranchLog(BranchLog branchLog)
        {
            _context.db.GetCollection<BranchLog>(_branchesLogCollectionName).InsertOne(branchLog);
        }

        public void AddCertificateLog(CertificateLog certificateLog)
        {
            _context.db.GetCollection<CertificateLog>(_certificatesLogCollectionName).InsertOne(certificateLog);
        }

        public void AddCompanyLog(CompanyLog companyLog)
        {
            _context.db.GetCollection<CompanyLog>(_companiesLogCollectionName).InsertOne(companyLog);
        }

        public void AddCourseLog(CourseLog courseLog)
        {
            _context.db.GetCollection<CourseLog>(_coursesLogCollectionName).InsertOne(courseLog);
        }

        public void AddCourseQueueLog(CourseQueueLog courseQueueLog)
        {
            _context.db.GetCollection<CourseQueueLog>(_coursesQueueLogCollectionName).InsertOne(courseQueueLog);
        }

        public void AddDegreeLog(DegreeLog degreeLog)
        {
            _context.db.GetCollection<DegreeLog>(_degreesLogCollectionName).InsertOne(degreeLog);
        }

        public void AddExamLog(ExamLog examLog)
        {
            _context.db.GetCollection<ExamLog>(_examsLogCollectionName).InsertOne(examLog);
        }

        public void AddExamResultLog(ExamResultLog examResultLog)
        {
            _context.db.GetCollection<ExamResultLog>(_examsResultsLogCollectionName).InsertOne(examResultLog);
        }

        public void AddExamTermLog(ExamTermLog examTermLog)
        {
            _context.db.GetCollection<ExamTermLog>(_examsTermsLogCollectionName).InsertOne(examTermLog);
        }

        public void AddGivenCertificateLog(GivenCertificateLog givenCertificateLog)
        {
            _context.db.GetCollection<GivenCertificateLog>(_givenCertificatesLogCollectionName).InsertOne(givenCertificateLog);
        }

        public void AddGivenDegreeLog(GivenDegreeLog givenDegreeLog)
        {
            _context.db.GetCollection<GivenDegreeLog>(_givenDegreesLogCollectionName).InsertOne(givenDegreeLog);
        }

        public void AddMeetingLog(MeetingLog meetingLog)
        {
            _context.db.GetCollection<MeetingLog>(_meetingsLogCollectionName).InsertOne(meetingLog);
        }

        public void AddUserLog(CertificationPlatformUserLog userLog)
        {
            _context.db.GetCollection<CertificationPlatformUserLog>(_usersLogCollectionName).InsertOne(userLog);
        }

        public void AddUserLoginLog(UserLoginLog userLoginLog)
        {
            _context.db.GetCollection<UserLoginLog>(_usersLoginLogCollectionName).InsertOne(userLoginLog);
        }




        #region PersonalUserLog
        public void AddLogToPersonalUserLogs(string userIdentificator, PersonalLogInformation logInfo)
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

        public ICollection<PersonalLog> GetPersonalUserLogsById(ICollection<string> usersIdentificators)
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
        #endregion
    }
}
