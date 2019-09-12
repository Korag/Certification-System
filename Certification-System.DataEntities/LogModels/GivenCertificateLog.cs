using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class GivenCertificateLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GivenCertificateLogIdentificator { get; set; }

        public GivenCertificate AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}