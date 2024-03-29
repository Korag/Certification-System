﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class Degree
    {
        public Degree()
        {
            Conditions = new List<string>();
            RequiredCertificates = new List<string>();
            RequiredDegrees = new List<string>();
            Branches = new List<string>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DegreeIdentificator { get; set; }

        public string DegreeIndexer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<string> Conditions { get; set; }
        public ICollection<string> RequiredCertificates { get; set; }
        public ICollection<string> RequiredDegrees { get; set; }
        public ICollection<string> Branches { get; set; }
    }
}