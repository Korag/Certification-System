using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class ExamTermLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamTermLogIdentificator { get; set; }

        public ExamTerm AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}