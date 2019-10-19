using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.ServicesInterfaces
{
    public interface ILogService
    {
        LogInformation GenerateLogInformation(string userEmailAddress, string actionName, string typeOfAction);
        LogInformation GenerateLogInformationByUserId(string userIdentificator, string actionName, string typeOfAction);
        UserLoginLogInformation GenerateUserLoginInformation(string userEmailAddress, string loginActionResult);
        void AddBranchLog(Branch branch, LogInformation logInfo);
        void AddBranchesLogs(ICollection<Branch> branches, LogInformation logInfo);
        void AddCertificateLog(Certificate certificate, LogInformation logInfo);
        void AddCertificatesLogs(ICollection<Certificate> certificates, LogInformation logInfo);
        void AddUserLog(CertificationPlatformUser user, LogInformation logInfo);
        void AddUsersLogs(ICollection<CertificationPlatformUser> users, LogInformation logInfo);
        void AddCompanyLog(Company company, LogInformation logInfo);
        void AddCompaniesLogs(ICollection<Company> companies, LogInformation logInfo);
        void AddCourseLog(Course course, LogInformation logInfo);
        void AddCoursesLogs(ICollection<Course> courses, LogInformation logInfo);
        void AddCourseQueueLog(CourseQueue courseQueue, LogInformation logInfo);
        void AddCoursesQueueLogs(ICollection<CourseQueue> coursesQueue, LogInformation logInfo);
        void AddDegreeLog(Degree degree, LogInformation logInfo);
        void AddDegreesLogs(ICollection<Degree> degrees, LogInformation logInfo);
        void AddExamLog(Exam exam, LogInformation logInfo);
        void AddExamsLogs(ICollection<Exam> exams, LogInformation logInfo);
        void AddExamResultLog(ExamResult examResult, LogInformation logInfo);
        void AddExamsResultsLogs(ICollection<ExamResult> examsResults, LogInformation logInfo);
        void AddExamTermLog(ExamTerm examTerm, LogInformation logInfo);
        void AddExamsTermsLogs(ICollection<ExamTerm> examsTerms, LogInformation logInfo);
        void AddGivenCertificateLog(GivenCertificate givenCertificate, LogInformation logInfo);
        void AddGivenCertificatesLogs(ICollection<GivenCertificate> givenCertificates, LogInformation logInfo);
        void AddGivenDegreeLog(GivenDegree givenDegree, LogInformation logInfo);
        void AddGivenDegreesLogs(ICollection<GivenDegree> givenDegrees, LogInformation logInfo);
        void AddMeetingLog(Meeting meeting, LogInformation logInfo);
        void AddMeetingsLogs(ICollection<Meeting> meetings, LogInformation logInfo);
        void AddUserLoginLog(UserLoginLogInformation logInfo);

        void AddPersonalUserLog(string userIdentificator, PersonalLogInformation logInfo);
        void AddPersonalUsersLogs(ICollection<string> usersIdentificators, PersonalLogInformation logInfo);
    }
}
