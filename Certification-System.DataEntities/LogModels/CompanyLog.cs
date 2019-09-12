using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class CompanyLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CompanyLogIdentificator { get; set; }

        public Company AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}