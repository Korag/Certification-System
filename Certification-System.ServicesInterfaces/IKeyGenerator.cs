using Certification_System.Entities;

namespace Certification_System.ServicesInterfaces
{
    public interface IKeyGenerator
    {
        string GenerateNewId();
        string GenerateNewGuid();
        string GenerateDeleteEntityCode(int codeLength);
        string GenerateUserTokenForEntityDeletion(CertificationPlatformUser user);
        bool ValidateUserTokenForEntityDeletion(CertificationPlatformUser user, string code);
        string GenerateCertificateEntityIndexer(Certificate certificate);
    }
}
