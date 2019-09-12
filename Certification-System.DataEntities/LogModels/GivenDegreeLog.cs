using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class GivenDegreeLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GivenDegreeLogIdentificator { get; set; }

        public GivenDegree AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}