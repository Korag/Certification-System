using Certification_System.Entities;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System;
using System.Security.Cryptography;

namespace Certification_System.Services
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly UserManager<CertificationPlatformUser> _userManager;

        private readonly string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";

        public KeyGenerator(UserManager<CertificationPlatformUser> userManager)
        {
            _userManager = userManager;
        }

        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateDeleteEntityCode(int codeLength)
        {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[codeLength];
                crypto.GetBytes(data);

                byte[] temporaryStorage = null;
                char[] generatedCode = new char[codeLength];

                int maxValue = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                for (int i = 0; i < codeLength; i++)
                {
                    byte currentValue = data[i];

                    while (currentValue > maxValue)
                    {
                        if (temporaryStorage == null)
                        {
                            temporaryStorage = new byte[1];
                        }

                        currentValue = temporaryStorage[0];
                        crypto.GetBytes(temporaryStorage);
                    }

                    generatedCode[i] = chars[currentValue % chars.Length];
                }

                return generatedCode.ToString();
            }
        }

        public string GenerateUserTokenForEntityDeletion(CertificationPlatformUser user)
        {
            return  _userManager.GenerateUserTokenAsync(user, "DeletionOfEntity", "DeletionOfEntity").Result;
        }

        public bool ValidateUserTokenForEntityDeletion(CertificationPlatformUser user, string code)
        {
            return _userManager.VerifyUserTokenAsync(user, "DeletionOfEntity", "DeletionOfEntity", code).Result;
        }
    }
}
