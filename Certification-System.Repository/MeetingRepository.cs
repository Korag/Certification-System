using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

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

        public ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators)
        {
            ICollection<Meeting> Meetings = new List<Meeting>();

            foreach (var meeting in meetingsIdentificators)
            {
                var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meeting);
                Meeting singleMeeting = _context.db.GetCollection<Meeting>(_meetingsCollectionName).Find<Meeting>(filter).FirstOrDefault();
                Meetings.Add(singleMeeting);
            }

            return Meetings;
        }

        public void AddMeeting(Meeting meeting)
        {
            _meetings = _context.db.GetCollection<Meeting>(_meetingsCollectionName);
            _meetings.InsertOne(meeting);
        }

        public void UpdateMeeting(Meeting meeting)
        {
            var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meeting.MeetingIdentificator);
            var result = _context.db.GetCollection<Meeting>(_meetingsCollectionName).ReplaceOne(filter, meeting);
        }

        public Meeting GetMeetingById(string meetingsIdentificators)
        {
            Meeting Meeting = new Meeting();

            var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meetingsIdentificators);
            Meeting = _context.db.GetCollection<Meeting>(_meetingsCollectionName).Find<Meeting>(filter).FirstOrDefault();

            return Meeting;
        }

        public ICollection<Meeting> GetMeetings()
        {
            _meetings = _context.db.GetCollection<Meeting>(_meetingsCollectionName);
            return _meetings.AsQueryable().ToList();
        }
    }
}
