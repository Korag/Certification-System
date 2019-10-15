using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ILogRepository
    {
        void AddBranchLog(BranchLog branchLog);
        void AddCertificateLog(CertificateLog certificateLog);
        void AddCompanyLog(CompanyLog companyLog);
        void AddCourseLog(CourseLog courseLog);
        void AddCourseQueueLog(CourseQueueLog courseQueueLog);
        void AddDegreeLog(DegreeLog degreeLog);
        void AddExamLog(ExamLog examLog);
        void AddExamResultLog(ExamResultLog examResultLog);
        void AddExamTermLog(ExamTermLog examTermLog);
        void AddGivenCertificateLog(GivenCertificateLog givenCertificateLog);
        void AddGivenDegreeLog(GivenDegreeLog givenDegreeLog);
        void AddMeetingLog(MeetingLog meetingLog);
        void AddUserLog(CertificationPlatformUserLog userLog);
        void AddUserLoginLog(UserLoginLog userLoginLog);
        void AddLogToPersonalUserLogs(string userIdentificator, LogInformation logInfo);


        ICollection<PersonalLog> GetListOfCourses();
        PersonalLog GetPersonalUserLogById(string userIdentificator);
        ICollection<PersonalLog> GetPersonalUserLogsById(ICollection<string> usersIdentificators);
        void CreatePersonalUserLog(CertificationPlatformUser user);
    }
}
