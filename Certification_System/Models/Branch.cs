using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Models
{
    public class Branch
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

    }
}