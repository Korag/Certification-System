using Certification_System.Entities;
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
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var degree in _degrees.AsQueryable().ToList())
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = degree.DegreeIndexer + " | " + degree.Name,
                            Value = degree.DegreeIdentificator
                        }
                    );
            };

            return SelectList;
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
            var filter = Builders<Degree>.Filter.Where(z => (z.RequiredCertificates.Intersect(givenCertificates).Count() == z.RequiredCertificates.Count()) && (z.RequiredDegrees.Intersect(givenDegrees).Count() == z.RequiredDegrees.Count()));
            var resultListOfGivenDegrees = GetDegrees().Find<Degree>(filter).ToList();

            return resultListOfGivenDegrees;
        }
    }
}
