using Certification_System.ServicesInterfaces;
using MongoDB.Bson;

namespace Certification_System.Services
{
    public class KeyGenerator : IKeyGenerator
    {
        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
