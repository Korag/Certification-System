﻿using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class DegreeRepository : IDegreeRepository
    {
        private readonly MongoContext _context;

        private readonly string _degreesCollectionName = "Degrees";
        private IMongoCollection<Degree> _degrees;

        public DegreeRepository(MongoContext context)
        {
            _context = context;
        }

        private IMongoCollection<Degree> GetDegrees()
        {
            return _degrees = _context.db.GetCollection<Degree>(_degreesCollectionName);
        }

        public ICollection<Degree> GetListOfDegrees()
        {
            return GetDegrees().AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetDegreesAsSelectList()
        {
            GetDegrees();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var degree in _degrees.AsQueryable().ToList())
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = degree.DegreeIndexer + " | " + degree.Name,
                            Value = degree.DegreeIdentificator
                        }
                    );
            };

            return selectList;
        }

        public void AddDegree(Degree degree)
        {
            GetDegrees().InsertOne(degree);
        }

        public Degree GetDegreeById(string degreeIdentificator)
        {
            var filter = Builders<Degree>.Filter.Eq(x => x.DegreeIdentificator, degreeIdentificator);
            var resultDegree = GetDegrees().Find<Degree>(filter).FirstOrDefault();

            return resultDegree;
        }

        public ICollection<Degree> GetDegreesById(ICollection<string> degreeIdentificators)
        {
            var filter = Builders<Degree>.Filter.Where(z => degreeIdentificators.Contains(z.DegreeIdentificator));
            var resultListOfDegrees = GetDegrees().Find<Degree>(filter).ToList();

            return resultListOfDegrees;
        }

        public void UpdateDegree(Degree degree)
        {
            var filter = Builders<Degree>.Filter.Eq(x => x.DegreeIdentificator, degree.DegreeIdentificator);
            var result = GetDegrees().ReplaceOne(filter, degree);
        }

        public ICollection<Degree> GetDegreesToDisposeByUserCompetences(ICollection<string> givenCertificates, ICollection<string> givenDegrees)
        {
            var degrees = GetListOfDegrees();

            List<Degree> availableDegreesToDispose = new List<Degree>();

            foreach (var degree in degrees)
            {
                if ((degree.RequiredCertificates.Intersect(givenCertificates).Count() == degree.RequiredCertificates.Count() && (degree.RequiredDegrees.Intersect(givenDegrees).Count() == degree.RequiredDegrees.Count())))
                {
                    availableDegreesToDispose.Add(degree);
                }
            }

            return availableDegreesToDispose;
        }

        public ICollection<Degree> DeleteBranchFromDegrees(string branchIdentificator)
        {
            var filter = Builders<Degree>.Filter.Where(z => z.Branches.Contains(branchIdentificator));
            var update = Builders<Degree>.Update.Pull(x => x.Branches, branchIdentificator);

            var resultListOfDegrees = GetDegrees().Find<Degree>(filter).ToList();
            resultListOfDegrees.ForEach(z => z.Branches.Remove(branchIdentificator));

            _degrees.UpdateMany(filter, update);

            return resultListOfDegrees;
        }

        public ICollection<Degree> DeleteRequiredCertificateFromDegrees(string certificateIdentificator)
        {
            var filter = Builders<Degree>.Filter.Where(z => z.RequiredCertificates.Contains(certificateIdentificator));
            var update = Builders<Degree>.Update.Pull(x => x.RequiredCertificates, certificateIdentificator);

            var resultListOfDegrees = GetDegrees().Find<Degree>(filter).ToList();
            resultListOfDegrees.ForEach(z => z.RequiredCertificates.Remove(certificateIdentificator));

            _degrees.UpdateMany(filter, update);

            return resultListOfDegrees;
        }

        public void DeleteDegree(string degreeIdentificator)
        {
            var filter = Builders<Degree>.Filter.Where(z => z.DegreeIdentificator == degreeIdentificator);
            var result = GetDegrees().DeleteOne(filter);
        }

        public ICollection<Degree> DeleteRequiredDegreeFromDegree(string degreeIdentificator)
        {
            var filter = Builders<Degree>.Filter.Where(z => z.RequiredDegrees.Contains(degreeIdentificator));
            var update = Builders<Degree>.Update.Pull(x => x.RequiredDegrees, degreeIdentificator);

            var resultListOfDegrees = GetDegrees().Find<Degree>(filter).ToList();
            resultListOfDegrees.ForEach(z => z.RequiredDegrees.Remove(degreeIdentificator));

            var result = _degrees.UpdateMany(filter, update);

            return resultListOfDegrees;
        }

        public string CountDegreesWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetDegrees().AsQueryable().Where(z => z.DegreeIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }
    }
}
