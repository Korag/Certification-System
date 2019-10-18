using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IPersonalLogRepository
    {
        ICollection<PersonalLog> GetListOfPersonalLogs();
        PersonalLog GetPersonalUserLogById(string userIdentificator);
        ICollection<PersonalLog> GetPersonalUserLogsById(ICollection<string> usersIdentificators);
        void CreatePersonalUserLog(CertificationPlatformUser user);
    }
}
