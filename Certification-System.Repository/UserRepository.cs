﻿using Certification_System.Entities;
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

        public ICollection<CertificationPlatformUser> GetListOfAdmins()
        {
            return GetUsers().AsQueryable().Where(z => z.Roles.Contains("ADMIN")).ToList();
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
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var dictionaryItem in UserRolesDictionary.TranslationDictionary)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = dictionaryItem.Value,
                    Value = dictionaryItem.Key
                };

                selectList.Add(item);
            }

            return selectList;
        }

        public ICollection<SelectListItem> GetAvailableRoleFiltersAsSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            SelectListItem noFilter = new SelectListItem()
            {
                Text = "Brak filtra",
                Value = ""
            };

            selectList.Add(noFilter);

            foreach (var dictionaryItem in UserRolesDictionary.TranslationDictionary)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = dictionaryItem.Value,
                    Value = dictionaryItem.Value
                };

                selectList.Add(item);
            }

            selectList.Last().Value = "Instruktor&Egzaminator";

            return selectList;
        }

        public ICollection<SelectListItem> GetAvailableCourseRoleFiltersAsSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>
            {
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

            return selectList;
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

        public ICollection<CertificationPlatformUser> GetUsersManagersByCompanyId(string companyIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.CompanyRoleManager.ElementAt(0) == companyIdentificator);
            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();

            return resultListOfUsers;
        }

        public ICollection<CertificationPlatformUser> GetUsersWorkersByCompanyId(string companyIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.CompanyRoleWorker.ElementAt(0) == companyIdentificator);
            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();

            return resultListOfUsers;
        }

        public List<SelectListItem> GenerateSelectList(ICollection<string> usersIdentificators)
        {
            var users = GetUsersById(usersIdentificators);

            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var user in users)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = user.FirstName + " " + user.LastName + " | " + user.Email,
                            Value = user.Id
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetInstructorsAsSelectList()
        {
            var instructors = GetListOfInstructors();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var instructor in instructors)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = instructor.FirstName + " " + instructor.LastName + " | " + instructor.Email,
                            Value = instructor.Id
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetExaminersAsSelectList()
        {
            var examiners = GetListOfExaminers();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var examiner in examiners)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = examiner.FirstName + " " + examiner.LastName + " | " + examiner.Email,
                            Value = examiner.Id
                        }
                    );
            };

            return selectList;
        }

        public ICollection<SelectListItem> GetWorkersAsSelectList()
        {
            var workers = GetListOfWorkers();
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var worker in workers)
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = worker.FirstName + " " + worker.LastName + " | " + worker.Email,
                            Value = worker.Id
                        }
                    );
            };

            return selectList;
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
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var user in _users.AsQueryable().ToList())
            {
                selectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = user.FirstName + " " + user.LastName + " | " + user.Email,
                            Value = user.Id
                        }
                    );
            };

            return selectList;
        }

        public void AddUserCertificate(string userIdentificator, string givenCertificateIdentificator)
        {
            var user = GetUserById(userIdentificator);
            user.GivenCertificates.Add(givenCertificateIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            GetUsers().ReplaceOne(filter, user);
        }

        public void AddUserDegree(string userIdentificator, string givenDegreeIdentificator)
        {
            var user = GetUserById(userIdentificator);
            user.GivenDegrees.Add(givenDegreeIdentificator);

            var filter = Builders<CertificationPlatformUser>.Filter.Eq(x => x.Id, userIdentificator);
            GetUsers().ReplaceOne(filter, user);
        }

        public CertificationPlatformUser AddUserToCourse(string courseIdentificator, string userIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => userIdentificator == z.Id);
            var update = Builders<CertificationPlatformUser>.Update.AddToSet(x => x.Courses, courseIdentificator);

            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();
            resultUser.Courses.Add(courseIdentificator);

            var result = _users.UpdateOne(filter, update);

            return resultUser;
        }

        public ICollection<CertificationPlatformUser> AddUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => usersIdentificators.Contains(z.Id));
            var update = Builders<CertificationPlatformUser>.Update.AddToSet(x => x.Courses, courseIdentificator);

            var resultListOfUsers = GetUsers().Find<CertificationPlatformUser>(filter).ToList();
            resultListOfUsers.ForEach(z => z.Courses.Add(courseIdentificator));

            var result = _users.UpdateMany(filter, update);

            return resultListOfUsers;
        }

        public CertificationPlatformUser GetUserByGivenCertificateId(string givenCertificateIdentificator)
        {
            var filter = Builders<CertificationPlatformUser>.Filter.Where(z => z.GivenCertificates.Contains(givenCertificateIdentificator));
            var resultUser = GetUsers().Find<CertificationPlatformUser>(filter).FirstOrDefault();

            return resultUser;
        }

        public ICollection<CertificationPlatformUser> GetUsersByGivenCertificatesId(ICollection<string> givenCertificatesIdentificators)
        {
            GetUsers();
            List<CertificationPlatformUser> resultListOfUsers = new List<CertificationPlatformUser>();

            foreach (var givenCertificateIdentificator in givenCertificatesIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.GivenCertificates.Contains(givenCertificateIdentificator));
                var resultUser = _users.Find<CertificationPlatformUser>(filter).FirstOrDefault();
                resultListOfUsers.Add(resultUser);
            }

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

        public ICollection<CertificationPlatformUser> GetUsersByGivenDegreesId(ICollection<string> givenDegreesIdentificators)
        {
            GetUsers();
            List<CertificationPlatformUser> resultListOfUsers = new List<CertificationPlatformUser>();

            foreach (var givenDegreeIdentificator in givenDegreesIdentificators)
            {
                var filter = Builders<CertificationPlatformUser>.Filter.Where(x => x.GivenDegrees.Contains(givenDegreeIdentificator));
                var resultUser = _users.Find<CertificationPlatformUser>(filter).FirstOrDefault();
                resultListOfUsers.Add(resultUser);
            }

            return resultListOfUsers;
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
