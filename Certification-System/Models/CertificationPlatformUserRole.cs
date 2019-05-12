using AspNetCore.Identity.Mongo.Model;

namespace Certification_System.Models
{
    public class CertificationPlatformUserRole : MongoRole
    {
        public CertificationPlatformUserRole()
        {

        }
        public CertificationPlatformUserRole(string name)
        {
            Name = name;
            NormalizedName = name.ToUpperInvariant();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
