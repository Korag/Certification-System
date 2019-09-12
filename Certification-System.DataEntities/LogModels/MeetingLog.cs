using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class MeetingLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MeetingLogIdentificator { get; set; }

        public Meeting AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}