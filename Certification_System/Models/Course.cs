using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Certification_System.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CourseIdentificator { get; set; }
        public string Name { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
        public int CourseLength { get; set; }
        public string Description { get; set; }

        public ICollection<string> Branches { get; set; }
        public ICollection<string> Meetings { get; set; }
    }
}