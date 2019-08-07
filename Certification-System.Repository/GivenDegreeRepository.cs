﻿using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class GivenDegreeRepository : IGivenDegreeRepository
    {
        private readonly MongoContext _context;

        private readonly string _givenDegreesCollectionName = "GivenDegrees";
        private IMongoCollection<GivenDegree> _givenDegrees;

        public GivenDegreeRepository()
        {
            _context = new MongoContext();
        }

        public ICollection<GivenDegree> GetGivenDegreesByIdOfDegree(string degreeIdentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.Degree, degreeIdentificator);
            var givenDegrees = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName).Find<GivenDegree>(filter).ToList();

            return givenDegrees;
        }

        public ICollection<GivenDegree> GetGivenDegrees()
        {
            _givenDegrees = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName);

            return _givenDegrees.AsQueryable().ToList();
        }

        public void AddGivenDegree(GivenDegree givenDegree)
        {
            _givenDegrees = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName);
            _givenDegrees.InsertOne(givenDegree);
        }

        public GivenDegree GetGivenDegreeById(string givenDegreeIdentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.GivenDegreeIdentificator, givenDegreeIdentificator);
            GivenDegree givenDegree = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName).Find<GivenDegree>(filter).FirstOrDefault();

            return givenDegree;
        }

        public void UpdateGivenDegree(GivenDegree givenDegree)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.GivenDegreeIdentificator, givenDegree.GivenDegreeIdentificator);
            var result = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName).ReplaceOne(filter, givenDegree);
        }

        public ICollection<GivenDegree> GetGivenDegreesById(ICollection<string> givenDegreeIdentificators)
        {
            List<GivenDegree> GivenDegrees = new List<GivenDegree>();

            foreach (var givenDegreeIdentificator in givenDegreeIdentificators)
            {
                GivenDegree singleGivenDegree= GetGivenDegreeById(givenDegreeIdentificator);
                GivenDegrees.Add(singleGivenDegree);
            }

            return GivenDegrees;
        }
    }
}
