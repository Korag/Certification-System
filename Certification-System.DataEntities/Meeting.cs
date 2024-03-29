﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class Meeting
    {
        public Meeting()
        {
            AttendanceList = new List<string>();
            Instructors = new List<string>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MeetingIdentificator { get; set; }

        public string MeetingIndexer { get; set; }
        public string Description { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string NumberOfApartment { get; set; }

        public ICollection<string> AttendanceList { get; set; }
        public ICollection<string> Instructors { get; set; }
    }
}