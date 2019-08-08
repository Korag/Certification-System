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

        private IMongoCollection<GivenDegree> GetGivenDegrees()
        {
            return _givenDegrees = _context.db.GetCollection<GivenDegree>(_givenDegreesCollectionName);
        }

        public ICollection<GivenDegree> GetListOfGivenDegrees()
        {
            return GetGivenDegrees().AsQueryable().ToList();
        }

        public ICollection<GivenDegree> GetGivenDegreesByIdOfDegree(string degreeIdentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.Degree, degreeIdentificator);
            var resultGivenDegrees = GetGivenDegrees().Find<GivenDegree>(filter).ToList();

            return resultGivenDegrees;
        }

        public void AddGivenDegree(GivenDegree givenDegree)
        {
            GetGivenDegrees().InsertOne(givenDegree);
        }

        public GivenDegree GetGivenDegreeById(string givenDegreeIdentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.GivenDegreeIdentificator, givenDegreeIdentificator);
            var resultGivenDegree = GetGivenDegrees().Find<GivenDegree>(filter).FirstOrDefault();

            return resultGivenDegree;
        }

        public void UpdateGivenDegree(GivenDegree givenDegree)
        {
            var filter = Builders<GivenDegree>.Filter.Eq(x => x.GivenDegreeIdentificator, givenDegree.GivenDegreeIdentificator);
            var result = GetGivenDegrees().ReplaceOne(filter, givenDegree);
        }

        public ICollection<GivenDegree> GetGivenDegreesById(ICollection<string> givenDegreeIdentificators)
        {
            var filter = Builders<GivenDegree>.Filter.Where(z => givenDegreeIdentificators.Contains(z.GivenDegreeIdentificator));
            var result = GetGivenDegrees().Find<GivenDegree>(filter).ToList();

            return result;
        }
    }
}
