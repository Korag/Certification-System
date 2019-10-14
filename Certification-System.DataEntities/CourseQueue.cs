using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class CourseQueue
    {
        public CourseQueue()
        {
            AwaitingUsers = new List<CourseQueueUser>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseIdentificator { get; set; }
        public ICollection<CourseQueueUser> AwaitingUsers { get; set; }
    }
}