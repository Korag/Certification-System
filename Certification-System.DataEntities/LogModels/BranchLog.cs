using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class BranchLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BranchLogIdentificator { get; set; }

        public Branch AlteredEntity { get; set; }

        public LogInformation LogData { get; set; }
    }
}