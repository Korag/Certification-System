using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Certification_System.Models
{
    public class GivenCertificate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GivenCertificateIdentificator { get; set; }

        public string GivenCertificateIndexer { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpirationDate{ get; set; }
        public bool Depreciated { get; set; }

        public string Course{ get; set; }
        public string Certificate { get; set; }
    }
}