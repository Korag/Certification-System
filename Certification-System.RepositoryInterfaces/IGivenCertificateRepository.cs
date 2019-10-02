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
        ICollection<GivenCertificate> DeleteGivenCertificatesByCertificateId(string certificateIdentificator);
        void DeleteGivenCertificate(string givenCertificateIdentificator);
        ICollection<GivenCertificate> DeleteGivenCertificatesByCourseId(string courseIdentificator);
        ICollection<GivenCertificate> DeleteGivenCertificates(ICollection<string> givenCertificatesIdentificators);
        string CountGivenCertificatesWithIndexerNamePart(string namePartOfIndexer);
        ICollection<GivenCertificate> GetGivenCertificatesByCourseId(string courseIdentificator);
    }
}
