﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class Certificate
    {
        public Certificate()
        {
            Branches = new List<string>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CertificateIdentificator { get; set; }

        public string CertificateIndexer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Depreciated { get; set; }

        public ICollection<string> Branches { get; set; }
    }
}