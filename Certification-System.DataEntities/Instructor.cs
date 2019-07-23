using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Certification_System.Entitities
{
    public class Instructor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InstructorIdentificator { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string NumberOfApartment { get; set; }
    }
}