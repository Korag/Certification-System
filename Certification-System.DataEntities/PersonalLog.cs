using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class PersonalLog
    {
        public PersonalLog()
        {
            LogData = new List<PersonalLogInformation>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserIdentificator { get; set; }

        public ICollection<PersonalLogInformation> LogData { get; set; }
    }
}