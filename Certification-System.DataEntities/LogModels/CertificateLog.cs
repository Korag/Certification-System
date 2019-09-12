using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class CertificateLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CertificateLogIdentificator { get; set; }

        public Certificate AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}