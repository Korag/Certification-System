﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Certification_System.Models
{
    public class GivenDegree
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GivenDegreeIdentificator { get; set; }

        public string GivenDegreeIndexer { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string Degree { get; set; }
    }
}