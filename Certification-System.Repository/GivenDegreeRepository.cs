using Certification_System.Entities;
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
    }
}
