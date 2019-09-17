using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

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
            var resultListOfGivenDegrees = GetGivenDegrees().Find<GivenDegree>(filter).ToList();

            return resultListOfGivenDegrees;
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
            var resultListOfGivenDegrees = GetGivenDegrees().Find<GivenDegree>(filter).ToList();

            return resultListOfGivenDegrees;
        }

        public void DeleteGivenDegree(string givenDegreeIdentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Where(z => z.GivenDegreeIdentificator == givenDegreeIdentificator);
            GetGivenDegrees().DeleteOne(filter);
        }

        public ICollection<GivenDegree> DeleteGivenDegreesByDegreeId(string degreedentificator)
        {
            var filter = Builders<GivenDegree>.Filter.Where(z => z.Degree == degreedentificator);

            var resultListOfGivenDegrees = GetGivenDegrees().Find<GivenDegree>(filter).ToList();
            var result = _givenDegrees.DeleteMany(filter);

            return resultListOfGivenDegrees;
        }

        public ICollection<GivenDegree> DeleteGivenDegrees(ICollection<string> givenDegreesIdentificators)
        {
            var filter = Builders<GivenDegree>.Filter.Where(z => givenDegreesIdentificators.Contains(z.GivenDegreeIdentificator));

            var resultListOfGivenDegrees = GetGivenDegrees().Find<GivenDegree>(filter).ToList();
            var result = _givenDegrees.DeleteMany(filter);

            return resultListOfGivenDegrees;
        }

        public string CountGivenDegreesWithIndexerNamePart(string namePartOfIndexer)
        {
            var indexersNumber = GetGivenDegrees().AsQueryable().Where(z => z.GivenDegreeIndexer.Contains(namePartOfIndexer)).Count();
            indexersNumber++;

            return indexersNumber.ToString();
        }
    }
}
