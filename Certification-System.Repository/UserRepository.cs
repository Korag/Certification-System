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

        private IMongoCollection<CertificationPlatformUser> GetUsers()
        {
            return _users = _context.db.GetCollection<CertificationPlatformUser>(_usersCollectionName);
        }

        public ICollection<CertificationPlatformUser> GetListOfInstructors()
        {
            return GetUsers().AsQueryable().Where(z => z.Roles.Contains("INSTRUCTOR")).ToList();
        }

        public ICollection<CertificationPlatformUser> GetListOfWorkers()
        {
            return GetUsers().AsQueryable().Where(z => z.Roles.Contains("WORKER")).ToList();
        }

        public ICollection<CertificationPlatformUser> GetListOfExaminers()
        {
            return GetUsers().AsQueryable().Where(z => z.Roles.Contains("EXAMINER")).ToList();
        }

        public ICollection<CertificationPlatformUser> GetListOfUsers()
        {
            return GetUsers().AsQueryable().ToList();
        }

        public ICollection<string> TranslateRoles(ICollection<string> userRoles)
        {
            List<string> translatedRoles = new List<string>();

            foreach (var role in userRoles)
            {
                translatedRoles.Add(UserRolesDictionary.TranslationDictionary[role]);
            }

            return translatedRoles;
        }

        public ICollection<SelectListItem> GetRolesAsSelectList()
        {
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var dictionaryItem in UserRolesDictionary.TranslationDictionary)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = dictionaryItem.Value,
                    Value = dictionaryItem.Key
                };

                SelectList.Add(item);
            }

            return SelectList;
        }

        public ICollection<SelectListItem> GetAvailableRoleFiltersAsSelectList()
        {
            List<SelectListItem> SelectList = new List<SelectListItem>();

            SelectListItem noFilter = new SelectListItem()
            {
                Text = "Brak filtra",
                Value = ""
            };

            SelectList.Add(noFilter);

            foreach (var dictionaryItem in UserRolesDictionary.TranslationDictionary)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = dictionaryItem.Value,
                    Value = dictionaryItem.Value
                };

                SelectList.Add(item);
            }

            SelectList.Last().Value = "Instruktor&Egzaminator";

            return SelectList;
        }

        public ICollection<SelectListItem> GetAvailableCourseRoleFiltersAsSelectList()
        {
            List<SelectListItem> SelectList = new List<SelectListItem>
            {
              //    new SelectListItem()
              //{
              //    Text = "Brak filtra",
              //    Value = ""
              //},
                  new SelectListItem()
              {
                  Text = UserRolesDictionary.TranslationDictionary["Instructor"],
                  Value = UserRolesDictionary.TranslationDictionary["Instructor"]
              },
                  new SelectListItem()
              {
                  Text = UserRolesDictionary.TranslationDictionary["Examiner"],
                  Value = UserRolesDictionary.TranslationDictionary["Examiner"]
              },
                  new SelectListItem()
              {
                  Text = UserRolesDictionary.TranslationDictionary["Instructor&Examiner"],
                  Value = "Instruktor&Egzaminator"
              }
            };

            return SelectList;
        }

        public CertificationPlatformUser GetUserById(string userIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            var result = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return result;
        }

        public CertificationPlatformUser GetInstructorById(string userIdentificator)
        {
            var result = GetListOfInstructors().Where(z => z.Id == userIdentificator).FirstOrDefault();

            return result;
        }

        public ICollection<SelectListItem> GetInstructorsAsSelectList()
        {
            var Instructors = GetListOfInstructors();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var instructor in Instructors)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = instructor.FirstName + " " + instructor.LastName + " | " + instructor.Email,
                            Value = instructor.Id
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetExaminersAsSelectList()
        {
            var Examiners = GetListOfExaminers();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var examiner in Examiners)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = examiner.FirstName + " " + examiner.LastName + " | " + examiner.Email,
                            Value = examiner.Id
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<SelectListItem> GetWorkersAsSelectList()
        {
            var Workers = GetListOfWorkers();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var worker in Workers)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = worker.FirstName + " " + worker.LastName + " | " + worker.Email,
                            Value = worker.Id
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<CertificationPlatformUser> GetInstructorsById(ICollection<string> userIdentificators)
        {
            var result = GetListOfInstructors().Where(z => userIdentificators.Contains(z.Id)).ToList();

            return result;
        }

        public ICollection<CertificationPlatformUser> GetUsersById(ICollection<string> userIdentificators)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => userIdentificators.Contains(z.Id));
            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();

            return resultListOfUsers;
        }

        public ICollection<SelectListItem> GetUsersAsSelectList()
        {
            GetUsers();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var user in _users.AsQueryable().ToList())
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
            User.GivenCertificates.Add(givenCertificateIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            GetUsers().ReplaceOne(filter, User);
        }

        public void AddUserDegree(string userIdentificator, string givenDegreeIdentificator)
        {
            var User = GetUserById(userIdentificator);
            User.GivenDegrees.Add(givenDegreeIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            GetUsers().ReplaceOne(filter, User);
        }

        public void AddUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => usersIdentificators.Contains(z.Id));
            var update = Builders<CertificationPlatformUser>.Update.AddToSet(x => x.Courses, courseIdentificator);

            var result = GetUsers().UpdateOne(filter, update);
        }

        public CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.GivenCertificates.Contains(givenCertificateIdentificator));
            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return resultUser;
        }

        public ICollection<CertificationPlatformUser> GetUsersByGivenCertificateId(ICollection<string> givenCertificatesIdentificators)
        {
            GetUsers();
            List<CertificationPlatformUser> resultListOfUsers = new List<CertificationPlatformUser>();

            foreach (var givenCertificateIdentificator in givenCertificatesIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.GivenCertificates.Contains(givenCertificateIdentificator));
                var resultUser = _users.Find<CertificationPlatformUser>(filter).FirstOrDefault();
                resultListOfUsers.Add(resultUser);
            }
            //var resultListOfUsers = GetUsers().AsQueryable().ToList().Where(z => z.Certificates.Where(x => givenCertificatesIdentificators.Contains(x)).Count() != 0).ToList();

            return resultListOfUsers;
        }


        public ICollection<CertificationPlatformUser> GetUsersConnectedToCompany(string companyIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.CompanyRoleManager.Contains(companyIdentificator) || x.CompanyRoleWorker.Contains(companyIdentificator));
            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();

            return resultListOfUsers;
        }

        public ICollection<CertificationPlatformUser> GetUsersByDegreeId(ICollection<string> degreeIdentificators)
        {
            GetUsers();
            List<CertificationPlatformUser> resultListOfUsers = new List<CertificationPlatformUser>();

            foreach (var degreeIdentificator in degreeIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.GivenDegrees.Contains(degreeIdentificator));
                var resultUser = _users.Find<CertificationPlatformUser>(filter).FirstOrDefault();
                resultListOfUsers.Add(resultUser);
            }

            return resultListOfUsers;
        }

        public CertificationPlatformUser GetUserByGivenDegreeId(string givenDegreeIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.GivenDegrees.Contains(givenDegreeIdentificator));
            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return resultUser;
        }

        public void UpdateUser(CertificationPlatformUser user)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, user.Id);
            var result = GetUsers().ReplaceOne(filter, user);
        }

        public void DeleteCourseFromUsersCollection(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => usersIdentificators.Contains(z.Id));
            var update = Builders<CertificationPlatformUser>.Update.Pull(x => x.Courses, courseIdentificator);

            //var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();
            //resultListOfUsers.ForEach(z => z.Courses.Remove(courseIdentificator));

            var result = GetUsers().UpdateMany(filter, update);
        }

        public CertificationPlatformUser GetUserByEmail(string emailAddress)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Email, emailAddress);
            var result = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return result;
        }

        public ICollection<CertificationPlatformUser> DeleteCompanyFromUsers(string companyIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.CompanyRoleManager.Contains(companyIdentificator) || z.CompanyRoleWorker.Contains(companyIdentificator));
            var filterRoleManager = Builders<CertificationPlatformUser>.Filter.Where(z => z.CompanyRoleManager.Contains(companyIdentificator));
            var filterRoleWorker = Builders<CertificationPlatformUser>.Filter.Where(z => z.CompanyRoleWorker.Contains(companyIdentificator));

            var updateRoleManager = Builders<CertificationPlatformUser>.Update.Pull(x => x.CompanyRoleManager, companyIdentificator);
            var updateRoleWorker = Builders<CertificationPlatformUser>.Update.Pull(x => x.CompanyRoleWorker, companyIdentificator);

            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();
            resultListOfUsers.ForEach(z => z.CompanyRoleManager.Remove(companyIdentificator));
            resultListOfUsers.ForEach(z => z.CompanyRoleWorker.Remove(companyIdentificator));

            _users.UpdateMany(filterRoleManager, updateRoleManager);
            _users.UpdateMany(filterRoleWorker, updateRoleWorker);

            return resultListOfUsers;
        }

        public CertificationPlatformUser DeleteUserGivenDegree(string givenDegreeIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.GivenDegrees.Contains(givenDegreeIdentificator));
            var update = Builders<CertificationPlatformUser>.Update.Pull(x => x.GivenDegrees, givenDegreeIdentificator);

            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();
            var result = _users.UpdateOne(filter, update);

            return resultUser;
        }

        public CertificationPlatformUser DeleteUserGivenCertificate(string givenCertificateIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.GivenCertificates.Contains(givenCertificateIdentificator));
            var update = Builders<CertificationPlatformUser>.Update.Pull(x => x.GivenCertificates, givenCertificateIdentificator);

            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();
            var result = _users.UpdateOne(filter, update);

            return resultUser;
        }

        public ICollection<CertificationPlatformUser> DeleteCourseFromUsers(string courseIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.Courses.Contains(courseIdentificator));
            var update = Builders<CertificationPlatformUser>.Update.Pull(x => x.Courses, courseIdentificator);

            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();
            resultListOfUsers.ForEach(z => z.Courses.Remove(courseIdentificator));

            var result = _users.UpdateMany(filter, update);

            return resultListOfUsers;
        }

        public void DeleteUser(string userIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.Id == userIdentificator);
            var result = GetUsers().DeleteOne(filter);
        }
    }
}
