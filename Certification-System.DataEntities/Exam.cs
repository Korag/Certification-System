using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class Exam
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExamIdentificator { get; set; }

        public string ExamIndexer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TypeOfExam { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public int OrdinalNumber { get; set; }
        public double MaxAmountOfPointsToEarn { get; set; }
        public int UsersLimit { get; set; }

        public bool ExamDividedToTerms { get; set; }
        public ICollection<string> ExamTerms { get; set; }

        public ICollection<string> Examiners { get; set; }
        public ICollection<string> ExamResults { get; set; }
        public ICollection<string> EnrolledUsers { get; set; }
    }
}