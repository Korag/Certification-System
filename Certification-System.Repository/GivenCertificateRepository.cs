﻿using Certification_System.Entities;
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
            var result = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();

            return result;
        }

        public ICollection<GivenCertificate> GetGivenCertificatesByIdOfCertificate(string certificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.Certificate, certificateIdentificator);
            var resultGivenCertificates = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();

            return resultGivenCertificates;
        }
    }
}
