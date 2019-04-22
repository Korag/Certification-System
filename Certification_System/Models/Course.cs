using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Certification_System.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public ICollection<string> Branches { get; set; }
        public ICollection<string> Meetings { get; set; }
    }
}