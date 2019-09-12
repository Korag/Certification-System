using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class DegreeLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DegreeLogIdentificator { get; set; }

        public Degree AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}