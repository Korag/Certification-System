using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Certification_System.Entities
{
    public class Exam
    {
        public Exam()
        {
            ExamTerms = new List<string>();
            Examiners = new List<string>();
            ExamResults = new List<string>();
            EnrolledUsers = new List<string>();
        }

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

        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string NumberOfApartment { get; set; }

        public bool ExamDividedToTerms { get; set; }
        public ICollection<string> ExamTerms { get; set; }

        public ICollection<string> Examiners { get; set; }
        public ICollection<string> ExamResults { get; set; }
        public ICollection<string> EnrolledUsers { get; set; }
    }
}