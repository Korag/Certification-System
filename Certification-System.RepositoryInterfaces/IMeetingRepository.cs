using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IMeetingRepository
    {
        ICollection<Meeting> GetListOfMeetings();
        ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators);
        Meeting GetMeetingById(string meetingsIdentificators);
        void AddMeeting(Meeting meeting);
        void UpdateMeeting(Meeting meeting);
        void ChangeUsersPresenceOnMeetings(string meetingIdentificator, ICollection<string> usersIdentificators);
        ICollection<Meeting> GetMeetingsByInstructorId(string userIdentificator);
        void AddMeetings(ICollection<Meeting> meetings);
        void UpdateMeetings(ICollection<Meeting> meetings);
        void DeleteMeeting(string meetingIdentificator);
        ICollection<Meeting> DeleteMeetings(ICollection<string> meetingsIdentificators);
        ICollection<Meeting> DeleteUserFromMeetings(string userIdentificator, ICollection<string> meetingsIdentificators);
    }
}
