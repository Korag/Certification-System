using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IGivenCertificateRepository
    {
        ICollection<GivenCertificate> GetListOfGivenCertificates();
        void AddGivenCertificate(GivenCertificate givenCertificate);
        void UpdateGivenCertificate(GivenCertificate givenCertificate);
        GivenCertificate GetGivenCertificateById(string givenCertificateIdentificator);
        ICollection<GivenCertificate> GetGivenCertificatesByIdOfCertificate(string certificateIdentificator);
        ICollection<GivenCertificate> GetGivenCertificatesById(ICollection<string> givenCertificatesIdentificators);
    }
}
