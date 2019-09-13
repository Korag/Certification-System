using AutoMapper;
using Certification_System.Entities;
using Certification_System.RepositoryInterfaces;
using Certification_System.ServicesInterfaces;
using System;

namespace Certification_System.Services
{
    public class LogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;

        private readonly IKeyGenerator _keyGenerator;

        public LogService(ILogRepository logRepository, IUserRepository userRepository, IKeyGenerator keyGenerator)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;

            _keyGenerator = keyGenerator;
        }

        public LogInformation GenerateLogInformation(string userEmailAddress, string typeOfAction)
        {
            var user = _userRepository.GetUserByEmail(userEmailAddress);

            return new LogInformation
            {
                ChangeAuthorEmail = user.Email,
                ChangeAuthorFirstName = user.FirstName,
                ChangeAuthorLastName = user.LastName,
                ChangeAuthorIdentificator = user.Id,

                DateTime = DateTime.Now,
                IpAddress = LocalIPGetter.GetLocalIPAddress(),
                TypeOfAction = typeOfAction
            };
        }

        public void AddBranchLog(Branch branch, LogInformation logInfo)
        {
            var branchLog = new BranchLog
            {
                BranchLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = branch,
                LogData = logInfo
            };

            _logRepository.AddBranchLog(branchLog);
        }

        public void AddCertificateLog(Certificate certificate, LogInformation logInfo)
        {
            var certificateLog = new CertificateLog
            {
                CertificateLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = certificate,
                LogData = logInfo
            };

            _logRepository.AddCertificateLog(certificateLog);
        }

        public void AddUserLog(CertificationPlatformUser user, LogInformation logInfo)
        {
            var userLog = new CertificationPlatformUserLog
            {
                CertificationPlatformUserLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = user,
                LogData = logInfo
            };

            _logRepository.AddUserLog(userLog);
        }

        public void AddCompanyLog(Company company, LogInformation logInfo)
        {
            var companyLog = new CompanyLog
            {
                CompanyLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = company,
                LogData = logInfo
            };

            _logRepository.AddCompanyLog(companyLog);
        }

        public void AddCourseLog(Course course, LogInformation logInfo)
        {
            var courseLog = new CourseLog
            {
                CourseLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = course,
                LogData = logInfo
            };

            _logRepository.AddCourseLog(courseLog);
        }

        public void AddDegreeLog(Degree degree, LogInformation logInfo)
        {
            var degreeLog = new DegreeLog
            {
                DegreeLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = degree,
                LogData = logInfo
            };

            _logRepository.AddDegreeLog(degreeLog);
        }

        public void AddExamLog(Exam exam, LogInformation logInfo)
        {
            var examLog = new ExamLog
            {
                ExamLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = exam,
                LogData = logInfo
            };

            _logRepository.AddExamLog(examLog);
        }

        public void AddExamResultLog(ExamResult examResult, LogInformation logInfo)
        {
            var examResultLog = new ExamResultLog
            {
                ExamResultLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = examResult,
                LogData = logInfo
            };

            _logRepository.AddExamResultLog(examResultLog);
        }

        public void AddExamTermLog(ExamTerm examTerm, LogInformation logInfo)
        {
            var examTermLog = new ExamTermLog
            {
                ExamTermLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = examTerm,
                LogData = logInfo
            };

            _logRepository.AddExamTermLog(examTermLog);
        }

        public void AddGivenCertificateLog(GivenCertificate givenCertificate, LogInformation logInfo)
        {
            var givenCertificateLog = new GivenCertificateLog
            {
                GivenCertificateLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = givenCertificate,
                LogData = logInfo
            };

            _logRepository.AddGivenCertificateLog(givenCertificateLog);
        }

        public void AddGivenDegreeResultLog(GivenDegree givenDegree, LogInformation logInfo)
        {
            var givenDegreeLog = new GivenDegreeLog
            {
                GivenDegreeLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = givenDegree,
                LogData = logInfo
            };

            _logRepository.AddGivenDegreeLog(givenDegreeLog);
        }

        public void AddMeetingLog(Meeting meeting, LogInformation logInfo)
        {
            var meetingLog = new MeetingLog
            {
                MeetingLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = meeting,
                LogData = logInfo
            };

            _logRepository.AddMeetingLog(meetingLog);
        }
    }
}
