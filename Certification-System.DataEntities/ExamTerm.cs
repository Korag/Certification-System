using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class ExamTerm
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamTermIdentificator { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public int DurationMinutes { get; set; }
        public int DurationDays { get; set; }
        public int UsersLimit { get; set; }

        public ICollection<string> EnrolledUsers { get; set; }
        public ICollection<string> Examinators { get; set; }
    }
}