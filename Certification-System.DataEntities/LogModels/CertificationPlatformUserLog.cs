using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class CertificationPlatformUserLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CertificationPlatformUserLogIdentificator { get; set; }

        public CertificationPlatformUser AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}
