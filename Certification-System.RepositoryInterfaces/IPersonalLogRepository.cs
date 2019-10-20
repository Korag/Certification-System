using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IPersonalLogRepository
    {
        void CreatePersonalUserLog(CertificationPlatformUser user);
        ICollection<RejectedUserFromCourseQueueLog> GetListOfRejectedUsers();
        ICollection<PersonalLog> GetListOfPersonalLogs();
        PersonalLog GetPersonalUserLogById(string userIdentificator);
        ICollection<PersonalLog> GetPersonalUsersLogsById(ICollection<string> usersIdentificators);
        PersonalLogInformation GeneratePersonalLogInformation(string userEmailAddress, string actionName, string typeOfAction, string urlToActionDetails, string descriptionOfAction);
        void AddPersonalUserLog(string userIdentificator, PersonalLogInformation logInfo);
        void AddPersonalUsersLogs(ICollection<string> usersIdentificators, PersonalLogInformation logInfo);
        void AddRejectedUserFromCourseQueueLog(CourseQueueWithSingleUser rejectedUser, LogInformation logInfo);
        void AddRejectedUsersFromCourseQueueLog(ICollection<CourseQueueWithSingleUser> rejectedUsers, LogInformation logInfo);
    }
}
