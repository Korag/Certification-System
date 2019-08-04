using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;

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

        public ICollection<Degree> GetDegrees()
        {
            _degrees = _context.db.GetCollection<Degree>(_degreesCollectionName);

            return _degrees.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetDegreesAsSelectList()
        {
            var Degrees = GetDegrees();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var degree in Degrees)
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
            _degrees = _context.db.GetCollection<Degree>(_degreesCollectionName);
            _degrees.InsertOne(degree);
        }

        public Degree GetDegreeById(string degreeIdentificator)
        {
            var filter = Builders<Degree>.Filter.Eq(x => x.DegreeIdentificator, degreeIdentificator);
            Degree degree = _context.db.GetCollection<Degree>(_degreesCollectionName).Find<Degree>(filter).FirstOrDefault();

            return degree;
        }

        public ICollection<Degree> GetDegreesById(ICollection<string> degreeIdentificators)
        {
            List<Degree> Degrees = new List<Degree>();

            if (degreeIdentificators != null)
            {
                foreach (var degreeIdentificator in degreeIdentificators)
                {
                    var filter = Builders<Degree>.Filter.Eq(x => x.DegreeIdentificator, degreeIdentificator);
                    Degree degree = _context.db.GetCollection<Degree>(_degreesCollectionName).Find<Degree>(filter).FirstOrDefault();

                    Degrees.Add(degree);
                }
            }

            return Degrees;
        }

        public void UpdateDegree(Degree degree)
        {
            var filter = Builders<Degree>.Filter.Eq(x => x.DegreeIdentificator, degree.DegreeIdentificator);
            var result = _context.db.GetCollection<Degree>(_degreesCollectionName).ReplaceOne(filter, degree);
        }
    }
}
