using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class PersonalLog
    {
        public PersonalLog()
        {
            LogData = new List<LogInformation>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserIdentificator { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<LogInformation> LogData { get; set; }
    }
}