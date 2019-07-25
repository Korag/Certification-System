using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IMeetingRepository
    {
        ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators);
        Meeting GetMeetingById(string meetingsIdentificators);
        void AddMeeting(Meeting meeting);
        void UpdateMeeting(Meeting meeting);
        ICollection<Meeting> GetMeetings();
    }
}
