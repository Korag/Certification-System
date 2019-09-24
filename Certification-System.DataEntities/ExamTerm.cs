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
        public string ExamTermIndexer { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string NumberOfApartment { get; set; }

        public int UsersLimit { get; set; }

        public ICollection<string> EnrolledUsers { get; set; }
        public ICollection<string> Examiners { get; set; }
    }
}