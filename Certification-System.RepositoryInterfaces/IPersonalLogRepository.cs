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
        PersonalLogInformation GeneratePersonalLogInformation(string userEmail, string actionName, string descriptionOfAction, string additionalInfo = "");
        void AddPersonalUserLog(string userIdentificator, PersonalLogInformation logInfo);
        void AddPersonalUsersLogs(ICollection<string> usersIdentificators, PersonalLogInformation logInfo);
        void AddRejectedUserLog(RejectedUserFromCourseQueueLog rejectedUser);
        void AddPersonalUsersLogsToAdminGroup(PersonalLogInformation logInfo);
    }
}
