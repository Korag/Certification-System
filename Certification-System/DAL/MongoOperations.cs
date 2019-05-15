using Certification_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Certification_System.DAL
{
    public class MongoOperations : IDatabaseOperations
    {
        private MongoContext _context;

        private string _usersCollectionName = "Users";
        private string _branchCollectionName = "Branches";
        private string _certificatesCollectionName = "Certificates";
        private string _coursesCollectionName = "Courses";
        private string _meetingsCollectionName = "Meetings";
        private string _instructorsCollectionName = "Instructors";
        private string _companiesCollectionName = "Companies";

        //Collections
        private IMongoCollection<CertificationPlatformUser> _users;
        private IMongoCollection<Branch> _branches;
        private IMongoCollection<Certificate> _certificates;
        private IMongoCollection<Course> _courses;
        private IMongoCollection<Meeting> _meetings;
        private IMongoCollection<Instructor> _instructors;
        private IMongoCollection<Company> _companies;

        public MongoOperations()
        {
            _context = new MongoContext();
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

        #region Branch
        public void AddBranch(Branch branch)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            _branches.InsertOne(branch);
        }

        public ICollection<Branch> GetBranches()
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            return _branches.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetBranchesAsSelectList()
        {
            var Branches = GetBranches();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var branch in Branches)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = branch.Name,
                            Value = branch.BranchIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<string> GetBranchesById(ICollection<string> branchesIdentificators)
        {
            _branches = _context.db.GetCollection<Branch>(_branchCollectionName);
            List<string> BranchesNames = new List<string>();

            foreach (var branch in branchesIdentificators)
            {
                var filter = Builders<Branch>.Filter.Eq(x => x.BranchIdentificator, branch);
                BranchesNames.Add(_branches.Find<Branch>(filter).Project(z => z.Name).FirstOrDefault());
            }

            return BranchesNames.AsQueryable().ToList();
        }
        #endregion

        #region Certificate     
        public ICollection<Certificate> GetCertificates()
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            return _certificates.AsQueryable().ToList();
        }

        public void AddCertificate(Certificate certificate)
        {
            _certificates = _context.db.GetCollection<Certificate>(_certificatesCollectionName);
            _certificates.InsertOne(certificate);
        }

        public Certificate GetCertificateById(string certificateIdentificator)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.CertificateIdentificator, certificateIdentificator);
            Certificate certificate = _context.db.GetCollection<Certificate>(_certificatesCollectionName).Find<Certificate>(filter).FirstOrDefault();
            return certificate;
        }

        public ICollection<SelectListItem> GetCertificatesAsSelectList()
        {
            List<Certificate> Certificates = GetCertificates().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var certificate in Certificates)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = certificate.CertificateIndexer + " " + certificate.Name,
                            Value = certificate.CertificateIdentificator
                        }
                    );
            };

            return SelectList;
        }

        #endregion

        #region Course
        public void AddCourse(Course course)
        {
            _courses = _context.db.GetCollection<Course>(_coursesCollectionName);
            _courses.InsertOne(course);
        }

        public Course GetCourseById(string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            return course;
        }
        public ICollection<Course> GetActiveCourses()
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseEnded, false);
            ICollection<Course> course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).ToList();
            return course;
        }

        public ICollection<SelectListItem> GetCoursesAsSelectList()
        {
            List<Course> Courses = GetActiveCourses().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();
            //SelectList.Add(new SelectListItem { Text = "---", Value = null });

            foreach (var course in Courses)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = course.CourseIndexer + " " + course.Name,
                            Value = course.CourseIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public Course GetCourseByMeetingId(string meetingIdentificator)
        {
            var filter = Builders<Course>.Filter.Where(x => x.Meetings.Contains(meetingIdentificator));
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            return course;
        }

        #endregion

        #region Meeting
        public ICollection<Meeting> GetMeetingsById(ICollection<string> meetingsIdentificators)
        {
            ICollection<Meeting> Meetings = new List<Meeting>();

            foreach (var meeting in meetingsIdentificators)
            {
                var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meeting);
                Meeting singleMeeting = _context.db.GetCollection<Meeting>(_meetingsCollectionName).Find<Meeting>(filter).FirstOrDefault();
                Meetings.Add(singleMeeting);
            }
            return Meetings;
        }

        public void AddMeeting(Meeting meeting)
        {
            _meetings = _context.db.GetCollection<Meeting>(_meetingsCollectionName);
            _meetings.InsertOne(meeting);
        }

        public Meeting GetMeetingById(string meetingsIdentificators)
        {
            Meeting Meeting = new Meeting();

            var filter = Builders<Meeting>.Filter.Eq(x => x.MeetingIdentificator, meetingsIdentificators);
            Meeting = _context.db.GetCollection<Meeting>(_meetingsCollectionName).Find<Meeting>(filter).FirstOrDefault();

            return Meeting;
        }

        public void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            course.Meetings.Add(meetingIdentificator);

            _context.db.GetCollection<Course>(_coursesCollectionName).ReplaceOne(filter, course);
        }

        #endregion

        #region Users
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
        #endregion

        #region Instructor
        public ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId)
        {
            List<Instructor> Instructors = new List<Instructor>();

            foreach (var instructor in InstructorsId)
            {
                var filter = Builders<Instructor>.Filter.Eq(x => x.InstructorIdentificator, instructor);
                Instructor singleInstructor = _context.db.GetCollection<Instructor>(_instructorsCollectionName).Find<Instructor>(filter).FirstOrDefault();
                Instructors.Add(singleInstructor);
            }
            return Instructors;
        }

        public Instructor GetInstructorById(string instructorIdentificator)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.InstructorIdentificator, instructorIdentificator);
            Instructor instructor = _context.db.GetCollection<Instructor>(_instructorsCollectionName).Find<Instructor>(filter).FirstOrDefault();
            return instructor;
        }

        public void AddInstructor(Instructor instructor)
        {
            _instructors = _context.db.GetCollection<Instructor>(_instructorsCollectionName);
            _instructors.InsertOne(instructor);
        }

        public ICollection<Instructor> GetInstructors()
        {
            _instructors = _context.db.GetCollection<Instructor>(_instructorsCollectionName);
            return _instructors.AsQueryable().ToList();
        }

        public ICollection<SelectListItem> GetInstructorsAsSelectList()
        {
            List<Instructor> Instructors = GetInstructors().ToList();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            foreach (var instructor in Instructors)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = instructor.FirstName + " " + instructor.LastName,
                            Value = instructor.InstructorIdentificator
                        }
                    );
            };

            return SelectList;
        }
        #endregion

        #region Company
        public ICollection<SelectListItem> GetCompaniesAsSelectList()
        {
            var Companies = GetCompanies();
            List<SelectListItem> SelectList = new List<SelectListItem>();
            //SelectList.Add(new SelectListItem { Text = "---", Value = null });

            foreach (var company in Companies)
            {
                SelectList.Add
                    (
                        new SelectListItem()
                        {
                            Text = company.CompanyName,
                            Value = company.CompanyIdentificator
                        }
                    );
            };

            return SelectList;
        }

        public ICollection<Company> GetCompanies()
        {
            _companies = _context.db.GetCollection<Company>(_companiesCollectionName);
            return _companies.AsQueryable().ToList();
        }

        public void AddCompany(Company company)
        {
            _companies = _context.db.GetCollection<Company>(_companiesCollectionName);
            _companies.InsertOne(company);
        }

        public Company GetCompanyById(string companyIdentificator)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, companyIdentificator);
            Company company = _context.db.GetCollection<Company>(_companiesCollectionName).Find<Company>(filter).FirstOrDefault();
            return company;
        }

        public ICollection<Company> GetCompaniesById(ICollection<string> companyIdentificators)
        {
            List<Company> Companies = new List<Company>();
            foreach (var companyIdentificator in companyIdentificators)
            {
                var filter = Builders<Company>.Filter.Eq(x => x.CompanyIdentificator, companyIdentificator);
                Company company = _context.db.GetCollection<Company>(_companiesCollectionName).Find<Company>(filter).FirstOrDefault();

                Companies.Add(company);
            }
            return Companies;
        }
        #endregion

        #region GivenCertificate
    
        public void AddGivenCertificate(GivenCertificate givenCertificate)
        {
      
        }

        #endregion

    }
}