using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class GivenCertificateRepository : IGivenCertificateRepository
    {
        private readonly MongoContext _context;

        private readonly string _givenCertificatesCollectionName = "GivenCertificates";
        private IMongoCollection<GivenCertificate> _givenCertificates;

        public GivenCertificateRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<GivenCertificate> GetGivenCertificates()
        {
            return _givenCertificates = _context.db.GetCollection<GivenCertificate>(_givenCertificatesCollectionName);
        }

        public ICollection<GivenCertificate> GetListOfGivenCertificates()
        {
            return GetGivenCertificates().AsQueryable().ToList();
        }

        public void AddGivenCertificate(GivenCertificate givenCertificate)
        {
            GetGivenCertificates().InsertOne(givenCertificate);
        }

        public void UpdateGivenCertificate(GivenCertificate givenCertificate)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.GivenCertificateIdentificator, givenCertificate.GivenCertificateIdentificator);
            var result = GetGivenCertificates().ReplaceOne(filter, givenCertificate);
        }

        public GivenCertificate GetGivenCertificateById(string givenCertificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.GivenCertificateIdentificator, givenCertificateIdentificator);
            var resultGivenCertificate = GetGivenCertificates().Find<GivenCertificate>(filter).FirstOrDefault();

            return resultGivenCertificate;
        }

        public ICollection<GivenCertificate> GetGivenCertificatesById(ICollection<string> givenCertificatesIdentificators)
        {
            var filter = Builders<GivenCertificate>.Filter.Where(z => givenCertificatesIdentificators.Contains(z.GivenCertificateIdentificator));
            var resultListOfGivenCertificate = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();

            return resultListOfGivenCertificate;
        }

        public ICollection<GivenCertificate> GetGivenCertificatesByIdOfCertificate(string certificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.Certificate, certificateIdentificator);
            var resultListOfGivenCertificate = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();

            return resultListOfGivenCertificate;
        }

        public ICollection<GivenCertificate> DeleteGivenCertificatesByCertificateId(string certificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Where(z => z.Certificate == certificateIdentificator);

            var resultListOfGivenCertificates = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();
            var result = _givenCertificates.DeleteMany(filter);

            return resultListOfGivenCertificates;
        }
    }
}
