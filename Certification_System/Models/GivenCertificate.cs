﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Certification_System.Models
{
    public class GivenCertificate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string ConcreteCertificateIdentificator { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime Expires { get; set; }

        public string Course{ get; set; }
        public string Certificate { get; set; }
    }
}