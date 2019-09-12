using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class ExamResultLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamResultLogIdentificator { get; set; }

        public ExamResult AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}