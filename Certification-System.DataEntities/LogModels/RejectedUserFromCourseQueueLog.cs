using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class RejectedUserFromCourseQueueLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RejectedUserFromCourseQueueLogIdentificator { get; set; }

        public CourseQueueWithSingleUser AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}