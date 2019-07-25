using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class CertificateRepository : ICertificateRepository
    {
        private MongoContext _context;

        private string _certificatesCollectionName = "Certificates";
        private IMongoCollection<Certificate> _certificates;

        public CertificateRepository(MongoContext context)
        {
            _context = context;
        }

        public ICollection<Certificate> GetCertificates()
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            return _certificates.AsQueryable().ToList();
        }

        public void AddCertificate(Certificate certificate)
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            _certificates.InsertOne(certificate);
        }

        public void UpdateCertificate(Certificate editedCertificate)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, editedCertificate.CertificateIdentificator);
            var result = _context.db.GetCollection<Certificate>(_certificatesCollectionName).ReplaceOne(filter, editedCertificate);
        }

        public Certificate GetCertificateById(string certificateIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, certificateIdentificator);
            Certificate certificate = _context.db.GetCollection<Certificate>(_certificatesCollectionName).Find<Certificate>(filter).FirstOrDefault();
            return certificate;
        }

        public ICollection<SelectListItem> GetCertificatesAsSelectList()
        {
            List<Certificate> Certificates = GetCertificates().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var certificate in Certificates)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = certificate.CertificateIndexer + " " + certificate.Name,
                            Value = certificate.CertificateIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<Certificate> GetCertificatesById(ICollection<string> certificateIdentificators)
        {
            List<Certificate> Certificates = new List<Certificate>();

            if (certificateIdentificators != null)
            {
                foreach (var certificateIdentificator in certificateIdentificators)
                {
                    var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, certificateIdentificator);
                    Certificate certificate = _context.db.GetCollection<Certificate>(_certificatesCollectionName).Find<Certificate>(filter).FirstOrDefault();

                    Certificates.Add(certificate);
                }
            }

            return Certificates;
        }
    }
}
