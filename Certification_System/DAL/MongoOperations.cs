using Certification_System.Models;
using MongoDB.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.DAL
{
    public class MongoOperations : IDatabaseOperations
    {
        private MongoContext _context;

        private string _usersCollectionName = "AspNetUsers";
        private string _branchCollectionName = "Branches";
        private string _certificatesCollectionName = "Certificates";
        private string _coursesCollectionName = "Courses";
        private string _meetingsCollectionName = "Meetings";
        private string _instructorsCollectionName = "Instructors";
        private string _companiesCollectionName = "Companies";

        //Collections
        private IMongoCollection<IdentityUser> _users;
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
                BranchesNames.Add(_branches.Find<Branch>(filter).Project(z=> z.Name).FirstOrDefault());
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

        #endregion

        #region Course
        public void AddCourse(Course course)
        {
            _courses = _context.db.GetCollection<Course>(_coursesCollectionName);
            _courses.InsertOne(course);
        }

        public Course GetCourseByCourId(string courseIdentificator)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.CourseIdentificator, courseIdentificator);
            Course course = _context.db.GetCollection<Course>(_coursesCollectionName).Find<Course>(filter).FirstOrDefault();
            return course;
        }
        #endregion

        #region Meeting
        public ICollection<Meeting> GetMeetingsById(ICollection<string> MeetingsId)
        {
            List<Meeting> Meetings = new List<Meeting>();

            foreach (var meeting in MeetingsId)
            {
                var filter = Builders<Meeting>.Filter.Eq(x => x.Id, meeting);
                Meeting singleMeeting = _context.db.GetCollection<Meeting>(_meetingsCollectionName).Find<Meeting>(filter).FirstOrDefault();
                Meetings.Add(singleMeeting);
            }
            return Meetings;
        }
        #endregion

        #region Users
        public ICollection<IdentityUser> GetUsers()
        {
            _users = _context.db.GetCollection<IdentityUser>(_usersCollectionName);
            return _users.AsQueryable().ToList();
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
        #endregion

        #region Company
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

        public Company GetCompanyByName(string companyName)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.CompanyName, companyName);
            Company company = _context.db.GetCollection<Company>(_companiesCollectionName).Find<Company>(filter).FirstOrDefault();
            return company;
        }
        #endregion

    }
}