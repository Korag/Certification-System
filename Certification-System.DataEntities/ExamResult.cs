using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Certification_System.Entities
{
    public class ExamResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamResultIdentificator { get; set; }
        public string ExamResultIndexer { get; set; }

        public string User { get; set; }

        public double PointsEarned { get; set; }
        public double PercentageOfResult { get; set; }
        public bool ExamPassed { get; set; }

        public string ExamTerm { get; set; }
    }
}