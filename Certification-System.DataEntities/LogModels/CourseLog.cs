using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class CourseLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseLogIdentificator { get; set; }

        public Course AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}