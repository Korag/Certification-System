using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IPersonalLogRepository
    {
        void CreatePersonalUserLog(CertificationPlatformUser user);
        ICollection<RejectedUserFromCourseQueueLog> GetListOfRejectedUsers();
        ICollection<PersonalLog> GetListOfPersonalLogs();
        ICollection<PersonalLog> GetListOfAdminPersonalLogs();
        PersonalLog GetPersonalUserLogById(string userIdentificator);
        ICollection<PersonalLog> GetPersonalUsersLogsById(ICollection<string> usersIdentificators);
        PersonalLogInformation GeneratePersonalLogInformation(string userEmail, string actionName, string descriptionOfAction, string additionalInfo = "");
        void AddPersonalUserLog(string userIdentificator, PersonalLogInformation logInfo);
        void AddPersonalUsersLogs(ICollection<string> usersIdentificators, PersonalLogInformation logInfo);
        void AddPersonalUsersMultipleLogs(ICollection<string> usersIdentificators, ICollection<PersonalLogInformation> logInfoCollection);
        void AddRejectedUserLog(RejectedUserFromCourseQueueLog rejectedUser);
        void AddPersonalUserLogToAdminGroup(PersonalLogInformation logInfo);
        void AddPersonalUsersLogsToAdminGroup(ICollection<PersonalLogInformation> logInfoCollection);
    }
}
