using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Certification_System.Models
{
    public class Meeting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime DateOfMeeting { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string NumberOfApartment { get; set; }

        public ICollection<string> AttendanceList { get; set; }
        public ICollection<string> InstructorId { get; set; }
    }
}