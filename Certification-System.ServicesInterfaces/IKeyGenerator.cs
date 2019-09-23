using Certification_System.Entities;

namespace Certification_System.ServicesInterfaces
{
    public interface IKeyGenerator
    {
        string GenerateNewId();
        string GenerateNewGuid();
        
        string GenerateUserTokenForEntityDeletion(CertificationPlatformUser user);
        bool ValidateUserTokenForEntityDeletion(CertificationPlatformUser user, string code);

        string GenerateCertificateEntityIndexer(string certificateName);
        string GenerateGivenCertificateEntityIndexer(string certificateIndexer);
        string GenerateDegreeEntityIndexer(string degreeName);
        string GenerateGivenDegreeEntityIndexer(string degreeIndexer);
        string GenerateCourseEntityIndexer(string courseName);
        string GenerateMeetingEntityIndexer(string courseIndexer);
        string GenerateExamEntityIndexer(string examName);
        string GenerateExamTermEntityIndexer(string examIndexer);
        string GenerateExamResultEntityIndexer(string examIndexer);
    }
}
