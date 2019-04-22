using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public string CourseId { get; set; }
        public string CertificateId { get; set; }
    }
}