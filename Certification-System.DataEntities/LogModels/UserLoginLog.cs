using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class UserLoginLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserLoginLogIdentificator { get; set; }

        public UserLoginLogInformation LogData { get; set; }
    }
}