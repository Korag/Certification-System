using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class ExamLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamLogIdentificator { get; set; }

        public Exam AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}