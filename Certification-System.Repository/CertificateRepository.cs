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
        private readonly MongoContext _context;

        private readonly string _certificatesCollectionName = "Certificates";
        private IMongoCollection<Certificate> _certificates;

        public CertificateRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<Certificate> GetCertificates()
        {
            return _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
        }

        public ICollection<Certificate> GetListOfCertificates()
        {
            return GetCertificates().AsQueryable().ToList();
        }

        public void AddCertificate(Certificate certificate)
        {
            GetCertificates().InsertOne(certificate);
        }

        public void UpdateCertificate(Certificate editedCertificate)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, editedCertificate.CertificateIdentificator);
            var result = GetCertificates().ReplaceOne(filter, editedCertificate);
        }

        public Certificate GetCertificateById(string certificateIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, certificateIdentificator);
            var resultCertificate = GetCertificates().Find<Certificate>(filter).FirstOrDefault();

            return resultCertificate;
        }

        public ICollection<SelectListItem> GetCertificatesAsSelectList()
        {
            GetCertificates();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var certificate in _certificates.AsQueryable().ToList())
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = certificate.CertificateIndexer + " | " + certificate.Name,
                            Value = certificate.CertificateIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<Certificate> GetCertificatesById(ICollection<string> certificateIdentificators)
        {
            var filter = Builders<Certificate>.Filter.Where(z => certificateIdentificators.Contains(z.CertificateIdentificator));
            var resultListOfCertificates = GetCertificates().Find<Certificate>(filter).ToList();

            return resultListOfCertificates;
        }

        public ICollection<Certificate> DeleteBranchFromCertificates(string branchIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Where(z => z.Branches.Contains(branchIdentificator));
            var update = Builders<Certificate>.Update.Pull(x => x.Branches, branchIdentificator);

            var resultListOfCertificates = GetCertificates().Find<Certificate>(filter).ToList();
            resultListOfCertificates.ForEach(z => z.Branches.Remove(branchIdentificator));

            _certificates.UpdateMany(filter, update);

            return resultListOfCertificates;
        }

        public void DeleteCertificate(string certificateIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Where(z => z.CertificateIdentificator == certificateIdentificator);
            GetCertificates().DeleteOne(filter);
        }
    }
}
