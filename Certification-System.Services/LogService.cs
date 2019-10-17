using Certification_System.Entities;
using Certification_System.RepositoryInterfaces;
using Certification_System.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Certification_System.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;

        private readonly IIPGetterService _ipGetter;
        private readonly IKeyGenerator _keyGenerator;

        public LogService(ILogRepository logRepository, IUserRepository userRepository, IKeyGenerator keyGenerator, IIPGetterService ipGetter)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;

            _ipGetter = ipGetter;
            _keyGenerator = keyGenerator;
        }

        public LogInformation GenerateLogInformation(string userEmailAddress, string actionName, string typeOfAction)
        {
            var user = _userRepository.GetUserByEmail(userEmailAddress);

            return new LogInformation
            {
                ChangeAuthorEmail = user.Email,
                ChangeAuthorFirstName = user.FirstName,
                ChangeAuthorLastName = user.LastName,
                ChangeAuthorIdentificator = user.Id,

                DateOfLogCreation = DateTime.Now,
                IpAddress = _ipGetter.GetGlobalIPAddress(),
                TypeOfAction = typeOfAction,

                ActionName = actionName,
            };
        }

        public LogInformation GenerateLogInformationByUserId(string userIdentificator, string actionName, string typeOfAction)
        {
            var user = _userRepository.GetUserById(userIdentificator);

            return new LogInformation
            {
                ChangeAuthorEmail = user.Email,
                ChangeAuthorFirstName = user.FirstName,
                ChangeAuthorLastName = user.LastName,
                ChangeAuthorIdentificator = user.Id,

                DateOfLogCreation = DateTime.Now,
                IpAddress = _ipGetter.GetGlobalIPAddress(),
                TypeOfAction = typeOfAction,

                ActionName = actionName,
            };
        }

        public UserLoginLogInformation GenerateUserLoginInformation(string userEmailAddress, string loginActionResult)
        {
            var user = _userRepository.GetUserByEmail(userEmailAddress);

            if (user != null)
            {
                return new UserLoginLogInformation
                {
                    UserEmail = user.Email,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserIdentificator = user.Id,

                    DateTime = DateTime.Now,
                    IpAddress = _ipGetter.GetGlobalIPAddress(),
                    ActionResult = loginActionResult,
                };
            }
            else
            {
                return new UserLoginLogInformation
                {
                    UserEmail = userEmailAddress,
                    UserFirstName = "EmailAddress not valid",
                    UserLastName = "EmailAddress not valid",
                    UserIdentificator = "EmailAddress not valid",

                    DateTime = DateTime.Now,
                    IpAddress = _ipGetter.GetGlobalIPAddress(),
                    ActionResult = loginActionResult,
                };
            }       
        }

        public void AddUserLoginLog(UserLoginLogInformation logInfo)
        {
            var userLoginLog = new UserLoginLog
            {
                UserLoginLogIdentificator = _keyGenerator.GenerateNewId(),
                LogData = logInfo
            };

            _logRepository.AddUserLoginLog(userLoginLog);
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

        public void AddBranchesLogs(ICollection<Branch> branches, LogInformation logInfo)
        {
            foreach (var branch in branches)
            {
                var branchLog = new BranchLog
                {
                    BranchLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = branch,
                    LogData = logInfo
                };

                _logRepository.AddBranchLog(branchLog);
            }
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

        public void AddCertificatesLogs(ICollection<Certificate> certificates, LogInformation logInfo)
        {
            foreach (var certificate in certificates)
            {
                var certificateLog = new CertificateLog
                {
                    CertificateLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = certificate,
                    LogData = logInfo
                };

                _logRepository.AddCertificateLog(certificateLog);
            }
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

        public void AddUsersLogs(ICollection<CertificationPlatformUser> users, LogInformation logInfo)
        {
            foreach (var user in users)
            {
                var userLog = new CertificationPlatformUserLog
                {
                    CertificationPlatformUserLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = user,
                    LogData = logInfo
                };

                _logRepository.AddUserLog(userLog);
            }
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

        public void AddCompaniesLogs(ICollection<Company> companies, LogInformation logInfo)
        {
            foreach (var company in companies)
            {
                var companyLog = new CompanyLog
                {
                    CompanyLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = company,
                    LogData = logInfo
                };

                _logRepository.AddCompanyLog(companyLog);
            }
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

        public void AddCoursesLogs(ICollection<Course> courses, LogInformation logInfo)
        {
            foreach (var course in courses)
            {
                var courseLog = new CourseLog
                {
                    CourseLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = course,
                    LogData = logInfo
                };

                _logRepository.AddCourseLog(courseLog);
            }
        }

        public void AddCourseQueueLog(CourseQueue courseQueue, LogInformation logInfo)
        {
            var courseQueueLog = new CourseQueueLog
            {
                CourseQueueLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = courseQueue,
                LogData = logInfo
            };

            _logRepository.AddCourseQueueLog(courseQueueLog);
        }

        public void AddCoursesQueueLogs(ICollection<CourseQueue> coursesQueue, LogInformation logInfo)
        {
            foreach (var courseQueue in coursesQueue)
            {
                var courseQueueLog = new CourseQueueLog
                {
                    CourseQueueLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = courseQueue,
                    LogData = logInfo
                };

                _logRepository.AddCourseQueueLog(courseQueueLog);
            }
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

        public void AddDegreesLogs(ICollection<Degree> degrees, LogInformation logInfo)
        {
            foreach (var degree in degrees)
            {
                var degreeLog = new DegreeLog
                {
                    DegreeLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = degree,
                    LogData = logInfo
                };

                _logRepository.AddDegreeLog(degreeLog);
            }
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

        public void AddExamsLogs(ICollection<Exam> exams, LogInformation logInfo)
        {
            foreach (var exam in exams)
            {
                var examLog = new ExamLog
                {
                    ExamLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = exam,
                    LogData = logInfo
                };

                _logRepository.AddExamLog(examLog);
            }
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

        public void AddExamsResultsLogs(ICollection<ExamResult> examsResults, LogInformation logInfo)
        {
            foreach (var examResult in examsResults)
            {
                var examResultLog = new ExamResultLog
                {
                    ExamResultLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = examResult,
                    LogData = logInfo
                };

                _logRepository.AddExamResultLog(examResultLog);
            }
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

        public void AddExamsTermsLogs(ICollection<ExamTerm> examsTerms, LogInformation logInfo)
        {
            foreach (var examTerm in examsTerms)
            {
                var examTermLog = new ExamTermLog
                {
                    ExamTermLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = examTerm,
                    LogData = logInfo
                };

                _logRepository.AddExamTermLog(examTermLog);
            }
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

        public void AddGivenCertificatesLogs(ICollection<GivenCertificate> givenCertificates, LogInformation logInfo)
        {
            foreach (var givenCertificate in givenCertificates)
            {
                var givenCertificateLog = new GivenCertificateLog
                {
                    GivenCertificateLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = givenCertificate,
                    LogData = logInfo
                };

                _logRepository.AddGivenCertificateLog(givenCertificateLog);
            }
        }

        public void AddGivenDegreeLog(GivenDegree givenDegree, LogInformation logInfo)
        {
            var givenDegreeLog = new GivenDegreeLog
            {
                GivenDegreeLogIdentificator = _keyGenerator.GenerateNewId(),
                AlteredEntity = givenDegree,
                LogData = logInfo
            };

            _logRepository.AddGivenDegreeLog(givenDegreeLog);
        }

        public void AddGivenDegreesLogs(ICollection<GivenDegree> givenDegrees, LogInformation logInfo)
        {
            foreach (var givenDegree in givenDegrees)
            {
                var givenDegreeLog = new GivenDegreeLog
                {
                    GivenDegreeLogIdentificator = _keyGenerator.GenerateNewId(),
                    AlteredEntity = givenDegree,
                    LogData = logInfo
                };

                _logRepository.AddGivenDegreeLog(givenDegreeLog);
            }
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

        public void AddMeetingsLogs(ICollection<Meeting> meetings, LogInformation logInfo)
        {
            foreach (var meeting in meetings)
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

        public void AddPersonalUserLog(string userIdentificator, LogInformation logInfo)
        {
            var userLog = _logRepository.GetPersonalUserLogById(userIdentificator);
            var user = _userRepository.GetUserById(userIdentificator);

            if (userLog == null)
            {
               _logRepository.CreatePersonalUserLog(user);
            }

            _logRepository.AddLogToPersonalUserLogs(userIdentificator, logInfo);
        }

        public void AddPersonalUsersLogs(ICollection<string> usersIdentificators, LogInformation logInfo)
        {
            foreach (var userIdentificator in usersIdentificators)
            {
                var user = _userRepository.GetUserById(userIdentificator);
                var userLog = _logRepository.GetPersonalUserLogById(userIdentificator);

                if (userLog == null)
                {
                   _logRepository.CreatePersonalUserLog(user);
                }

                _logRepository.AddLogToPersonalUserLogs(userIdentificator, logInfo);
            }
        }
    }
}
