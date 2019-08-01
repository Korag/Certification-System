﻿using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Certification_System.Repository
{
    public class GivenDegreeRepository : IGivenDegreeRepository
    {
        private MongoContext _context;

        private string _givenDegreesCollectionName = "GivenDegrees";
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
    }
}