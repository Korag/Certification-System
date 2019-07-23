using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certification_System.Entitities
{
    public class CertificationPlatformUser : MongoUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string Address { get; set; }

        public string NumberOfApartment { get; set; }

        public string DateOfBirth { get; set; }

        public ICollection<string> CompanyRoleManager { get; set; }
        public ICollection<string> CompanyRoleWorker { get; set; }
        public ICollection<string> Courses { get; set; }
        public ICollection<string> Certificates { get; set; }
        public ICollection<string> Degrees { get; set; }
    }
}
