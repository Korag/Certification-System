using AspNetCore.Identity.Mongo.Model;
using System.Collections.Generic;

namespace Certification_System.Entities
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
        public ICollection<string> GivenCertificates { get; set; }
        public ICollection<string> GivenDegrees { get; set; }
    }
}
