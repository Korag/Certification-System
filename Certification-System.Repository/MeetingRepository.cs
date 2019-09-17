using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly MongoContext _context;

        private readonly string _meetingsCollectionName = "Meetings";
        private IMongoCollection<Meeting> _meetings;

        public MeetingRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<Meeting> GetMeetings()
        {
            return _meetings = _context.db.GetCollection<Meeting>(_meetingsCollectionName);
        }

        public ICollection<Meeting> GetListOfMeetings()
        {
            return GetMeetings().AsQueryable().ToList();
        }

        public ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators)
        {
            var filter = Builders<Meeting>.Filter.Where(z => meetingsIdentificators.Contains(z.MeetingIdentificator));
            var resultListOfMeetings = GetMeetings().Find<Meeting>(filter).ToList();

            return resultListOfMeetings;
        }

        public void AddMeeting(Meeting meeting)
        {
            GetMeetings().InsertOne(meeting);
        }

        public void UpdateMeeting(Meeting meeting)
        {
            var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meeting.MeetingIdentificator);
            var result = GetMeetings().ReplaceOne(filter, meeting);
        }

        public Meeting GetMeetingById(string meetingsIdentificators)
        {
            var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meetingsIdentificators);
            var resultMeeting = GetMeetings().Find<Meeting>(filter).FirstOrDefault();

            return resultMeeting;
        }

        public void ChangeUsersPresenceOnMeetings(string meetingIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<Meeting>.Filter.Where(z => z.MeetingIdentificator == meetingIdentificator);
            var clearCollection = Builders<Meeting>.Update.Unset(x => x.AttendanceList);
            var update = Builders<Meeting>.Update.AddToSetEach(x => x.AttendanceList, usersIdentificators);

            var resultClear = GetMeetings().UpdateOne(filter, clearCollection);
            var resultUpdate = GetMeetings().UpdateOne(filter, update);
        }

        public ICollection<Meeting> GetMeetingsByInstructorId(string userIdentificator)
        {
            var filter = Builders<Meeting>.Filter.Where(z=> z.Instructors.Contains(userIdentificator));
            var resultListOfMeetings = GetMeetings().Find<Meeting>(filter).ToList();

            return resultListOfMeetings;
        }

        public void AddMeetings(ICollection<Meeting> meetings)
        {
            GetMeetings().InsertMany(meetings);
        }

        public void UpdateMeetings(ICollection<Meeting> meetings)
        {
            foreach (var meeting in meetings)
            {
                var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meeting.MeetingIdentificator);
                var result = GetMeetings().ReplaceOne(filter, meeting);
            }
        }

        public void DeleteMeeting(string meetingIdentificator)
        {
            var filter = Builders<Meeting>.Filter.Where(z => z.MeetingIdentificator == meetingIdentificator);
            var result = GetMeetings().DeleteOne(filter);
        }

        public ICollection<Meeting> DeleteMeetings(ICollection<string> meetingsIdentificators)
        {
            var filter = Builders<Meeting>.Filter.Where(z => meetingsIdentificators.Contains(z.MeetingIdentificator));

            var resultListOfMeetings = GetMeetings().Find<Meeting>(filter).ToList();
            var result = _meetings.DeleteMany(filter);

            return resultListOfMeetings;
        }

        public ICollection<Meeting> DeleteUserFromMeetings(string userIdentificator, ICollection<string> meetingIdentificators = null)
        {
            var filter = Builders<Meeting>.Filter.Where(z => meetingIdentificators.Contains(z.MeetingIdentificator) && (z.AttendanceList.Contains(userIdentificator) || z.Instructors.Contains(userIdentificator)));
            var update = Builders<Meeting>.Update.Pull(x => x.AttendanceList, userIdentificator).Pull(x=> x.Instructors, userIdentificator);

            var resultListOfMeetings = GetMeetings().Find<Meeting>(filter).ToList();
            resultListOfMeetings.ForEach(z => z.AttendanceList.Remove(userIdentificator));
            resultListOfMeetings.ForEach(z => z.Instructors.Remove(userIdentificator));

            var result = _meetings.UpdateMany(filter, update);

            return resultListOfMeetings;
        }

        public string CountMeetingsWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetMeetings().AsQueryable().Where(z => z.MeetingIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }
    }
}
