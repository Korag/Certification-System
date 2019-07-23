using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entitities
{
    public class Branch
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BranchIdentificator { get; set; }

        public string Name { get; set; }

    }
}