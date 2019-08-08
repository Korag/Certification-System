using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ICertificateRepository
    {
        ICollection<Certificate> GetListOfCertificates();
        void AddCertificate(Certificate certificate);
        void UpdateCertificate(Certificate editedCertificate);
        Certificate GetCertificateById(string certificateIdentificator);
        ICollection<SelectListItem> GetCertificatesAsSelectList();
        ICollection<Certificate> GetCertificatesById(ICollection<string> certificateIdentificators);
    }
}
