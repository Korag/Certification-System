using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class GivenCertificateRepository : IGivenCertificateRepository
    {
        private MongoContext _context;

        private string _givenCertificatesCollectionName = "GivenCertificates";
        private IMongoCollection<GivenCertificate> _givenCertificates;

        public GivenCertificateRepository(MongoContext context)
        {
            _context = context;
        }

        public void AddGivenCertificate(GivenCertificate givenCertificate)
        {
            _givenCertificates = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName);
            _givenCertificates.InsertOne(givenCertificate);
        }

        public void UpdateGivenCertificate(GivenCertificate givenCertificate)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.GivenCertificateIdentificator, givenCertificate.GivenCertificateIdentificator);
            var result = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName).ReplaceOne(filter, givenCertificate);
        }

        public GivenCertificate GetGivenCertificateById(string givenCertificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.GivenCertificateIdentificator, givenCertificateIdentificator);
            GivenCertificate givenCertificate = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName).Find<GivenCertificate>(filter).FirstOrDefault();
            return givenCertificate;
        }

        public ICollection<GivenCertificate> GetGivenCertificatesById(ICollection<string> givenCertificatesIdentificators)
        {
            List<GivenCertificate> GivenCertificates = new List<GivenCertificate>();

            foreach (var givenCertificateIdentificator in givenCertificatesIdentificators)
            {
                var filter = Builders<GivenCertificate>.Filter.Eq(x => x.GivenCertificateIdentificator, givenCertificateIdentificator);
                GivenCertificate singleGivenCertificate = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName).Find<GivenCertificate>(filter).FirstOrDefault();
                GivenCertificates.Add(singleGivenCertificate);
            }
            return GivenCertificates;
        }

        public ICollection<GivenCertificate> GetGivenCertificates()
        {
            _givenCertificates = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName);
            return _givenCertificates.AsQueryable().ToList();
        }

        public ICollection<GivenCertificate> GetGivenCertificatesByIdOfCertificate(string certificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.Certificate, certificateIdentificator);
            var givenCertificates = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName).Find<GivenCertificate>(filter).ToList();
            return givenCertificates;
        }
    }
}
