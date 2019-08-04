using Certification_System.ServicesInterfaces;
using MongoDB.Bson;
using System;

namespace Certification_System.Services
{
    public class KeyGenerator : IKeyGenerator
    {
        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
