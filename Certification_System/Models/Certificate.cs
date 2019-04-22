﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Models
{
    public class Certificate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CertificateIdentificator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<string> Branches { get; set; }
    }
}