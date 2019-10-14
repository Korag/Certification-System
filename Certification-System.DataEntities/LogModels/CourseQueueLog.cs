using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class CourseQueueLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseQueueLogIdentificator { get; set; }

        public CourseQueue AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}