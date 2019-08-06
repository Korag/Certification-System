using Certification_System.Entities;
using Certification_System.Repository.Context;
using Certification_System.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoContext _context;

        private readonly string _usersCollectionName = "Users";
        private IMongoCollection<CertificationPlatformUser> _users;

        public UserRepository(MongoContext context)
        {
            _context = context;
        }

        public ICollection<SelectListItem> GetRolesAsSelectList()
        {
            List<SelectListItem> SelectList = new List<SelectListItem>
            {
                 new SelectListItem()
                        {
                            Text = "Worker",
                            Value = "Worker"
                        },
                  new SelectListItem()
                        {
                            Text = "Company",
                            Value = "Company"
                        },
                   new SelectListItem()
                        {
                            Text = "Admin",
                            Value = "Admin"
                        }
            };

            return SelectList;
        }

        public ICollection<CertificationPlatformUser> GetUsers()
        {
            _users = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName);

            return _users.AsQueryable().ToList();
        }

        public CertificationPlatformUser GetUserById(string userIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            CertificationPlatformUser user = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return user;
        }

        public ICollection<CertificationPlatformUser> GetUsersById(ICollection<string> userIdentificators)
        {
            List<CertificationPlatformUser> Users = new List<CertificationPlatformUser>();

            foreach (var user in userIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, user);
                CertificationPlatformUser singleUser = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();
                Users.Add(singleUser);
            }

            return Users;
        }

        public ICollection<SelectListItem> GetUsersAsSelectList()
        {
            List<CertificationPlatformUser> Users = GetUsers().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var user in Users)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = user.FirstName + " " + user.LastName + " | " + user.Email,
                            Value = user.Id
                        }
                    );
            };

            return SelectList;
        }

        public void AddUserCertificate(string userIdentificator, string givenCertificateIdentificator)
        {
            var User = GetUserById(userIdentificator);
            User.Certificates.Add(givenCertificateIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);

            _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).ReplaceOne(filter, User);
        }

        public void AddUserDegree(string userIdentificator, string givenDegreeIdentificator)
        {
            var User = GetUserById(userIdentificator);
            User.Degrees.Add(givenDegreeIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);

            _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).ReplaceOne(filter, User);
        }

        public CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.Certificates.Contains(givenCertificateIdentificator));
            CertificationPlatformUser user = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return user;
        }

        public ICollection<CertificationPlatformUser> GetUsersByGivenCertificateId(ICollection<string> givenCertificatesIdentificators)
        {
            List<CertificationPlatformUser> Users = new List<CertificationPlatformUser>();

            foreach (var givenCertificateIdentificator in givenCertificatesIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.Certificates.Contains(givenCertificateIdentificator));
                CertificationPlatformUser singleUser = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();
                Users.Add(singleUser);
            }

            return Users;
        }

        public ICollection<CertificationPlatformUser> GetUsersConnectedToCompany(string companyIdentificator)
        {
            List<CertificationPlatformUser> Users = new List<CertificationPlatformUser>();

            var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.CompanyRoleManager.Contains(companyIdentificator) || x.CompanyRoleWorker.Contains(companyIdentificator));
            Users = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).ToList();

            return Users;
        }

        public ICollection<CertificationPlatformUser> GetUsersByDegreeId(ICollection<string> degreeIdentificators)
        {
            List<CertificationPlatformUser> Users = new List<CertificationPlatformUser>();

            foreach (var degreeIdentificator in degreeIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.Degrees.Contains(degreeIdentificator));
                CertificationPlatformUser singleUser = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();
                Users.Add(singleUser);
            }

            return Users;
        }

        public CertificationPlatformUser GetUserByGivenDegreeId(string givenDegreeIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.Degrees.Contains(givenDegreeIdentificator));
            CertificationPlatformUser user = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return user;
        }

        public void UpdateUser(CertificationPlatformUser user)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, user.Id);
            var result = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName).ReplaceOne(filter, user);
        }
    }
}
