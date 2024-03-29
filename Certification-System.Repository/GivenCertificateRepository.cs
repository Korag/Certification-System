﻿using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

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

        public ICollection<GivenCertificate> GetGivenCertificatesByCourseId(string courseIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Eq(x => x.Course, courseIdentificator);
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

        public ICollection<GivenCertificate> DeleteGivenCertificatesByCourseId(string courseIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Where(z => z.Course == courseIdentificator);

            var resultListOfGivenCertificates = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();
            var result = _givenCertificates.DeleteMany(filter);

            return resultListOfGivenCertificates;
        }

        public void DeleteGivenCertificate(string givenCertificateIdentificator)
        {
            var filter = Builders<GivenCertificate>.Filter.Where(z => z.GivenCertificateIdentificator == givenCertificateIdentificator);
            var result = GetGivenCertificates().DeleteOne(filter);
        }

        public ICollection<GivenCertificate> DeleteGivenCertificates(ICollection<string> givenCertificatesIdentificators)
        {
            var filter = Builders<GivenCertificate>.Filter.Where(z => givenCertificatesIdentificators.Contains(z.GivenCertificateIdentificator));

            var resultListOfGivenCertificates = GetGivenCertificates().Find<GivenCertificate>(filter).ToList();
            var result = GetGivenCertificates().DeleteMany(filter);

            return resultListOfGivenCertificates;
        }

        public string CountGivenCertificatesWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetGivenCertificates().AsQueryable().Where(z => z.GivenCertificateIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }
    }
}
