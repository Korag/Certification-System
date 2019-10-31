using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Text;

namespace Certification_System.Services
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly UserManager<CertificationPlatformUser> _userManager;
        private readonly MongoOperations _context;

        public KeyGenerator(UserManager<CertificationPlatformUser> userManager, MongoOperations context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateRandomFourDigitToken()
        {
            Random randomize = new Random();

            string generatedToken = "";

            for (int i = 0; i < 4; i++)
            {
                generatedToken += randomize.Next(0,9).ToString();
            }

            return generatedToken;
        }

        public string GenerateUserTokenForEntityDeletion(CertificationPlatformUser user)
        {
            return _userManager.GenerateUserTokenAsync(user, "DeletionOfEntity", "DeletionOfEntity").Result;
        }

        public bool ValidateUserTokenForEntityDeletion(CertificationPlatformUser user, string code)
        {
            return _userManager.VerifyUserTokenAsync(user, "DeletionOfEntity", "DeletionOfEntity", code).Result;
        }

        public string GenerateUserTokenForAssignToCourseQueuePurpouse(CertificationPlatformUser user)
        {
            return _userManager.GenerateUserTokenAsync(user, "AssignToCourseQueue", "AssignToCourseQueue").Result;
        }

        public bool ValidateUserTokenForAssignToCourseQueuePurpouse(CertificationPlatformUser user, string code)
        {
            return _userManager.VerifyUserTokenAsync(user, "AssignToCourseQueue", "AssignToCourseQueue", code).Result;
        }

        public string GenerateCertificateEntityIndexer(string certificateName)
        {
            StringBuilder certificateIndexer = new StringBuilder();

            certificateName.ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => certificateIndexer.Append(i[0]));

            if (certificateIndexer.Length < 5)
            {
                certificateIndexer.Append("CTCTCT");
            }
            if (certificateIndexer.Length > 5)
            {
                certificateIndexer.Remove(5, certificateIndexer.Length - 5);
            }

            var numerator = _context.certificateRepository.CountCertificatesWithIndexerNamePart(certificateIndexer.ToString());
            certificateIndexer.Append(numerator);

            if (certificateIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - certificateIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                certificateIndexer.Insert(certificateIndexer.Length - numerator.Length, fillByZero);
            }

            certificateIndexer.Append("CT");

            return certificateIndexer.ToString();
        }

        public string GenerateGivenCertificateEntityIndexer(string certificateIndexer)
        {
            StringBuilder givenCertificateIndexer = new StringBuilder();

            var splittedCertificateIndexer = certificateIndexer.ToUpper().Split("-", 5, StringSplitOptions.RemoveEmptyEntries).ToList();
            givenCertificateIndexer.Append(splittedCertificateIndexer[0]);

            var numerator = _context.givenCertificateRepository.CountGivenCertificatesWithIndexerNamePart(givenCertificateIndexer.ToString());
            givenCertificateIndexer.Append(numerator);

            if (givenCertificateIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - givenCertificateIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                givenCertificateIndexer.Insert(givenCertificateIndexer.Length - numerator.Length, fillByZero);
            }

            givenCertificateIndexer.Append("GCT");

            return givenCertificateIndexer.ToString();
        }

        public string GenerateDegreeEntityIndexer(string degreeName)
        {
            StringBuilder degreeIndexer = new StringBuilder();

            degreeName.ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => degreeIndexer.Append(i[0]));

            if (degreeIndexer.Length < 5)
            {
                degreeIndexer.Append("DG");
            }
            if (degreeIndexer.Length > 5)
            {
                degreeIndexer.Remove(5, degreeIndexer.Length - 5);
            }

            var numerator = _context.degreeRepository.CountDegreesWithIndexerNamePart(degreeIndexer.ToString());
            degreeIndexer.Append(numerator);

            if (degreeIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - degreeIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                degreeIndexer.Insert(degreeIndexer.Length - numerator.Length, fillByZero);
            }

            degreeIndexer.Append("DG");

            return degreeIndexer.ToString();
        }

        public string GenerateGivenDegreeEntityIndexer(string degreeIndexer)
        {
            StringBuilder givenDegreeIndexer = new StringBuilder();

            var splittedDegreeIndexer = degreeIndexer.ToUpper().Split("-", 5, StringSplitOptions.RemoveEmptyEntries).ToList();
            givenDegreeIndexer.Append(splittedDegreeIndexer[0]);

            var numerator = _context.givenDegreeRepository.CountGivenDegreesWithIndexerNamePart(givenDegreeIndexer.ToString());
            givenDegreeIndexer.Append(numerator);

            if (givenDegreeIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - givenDegreeIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                givenDegreeIndexer.Insert(givenDegreeIndexer.Length - numerator.Length, fillByZero);
            }

            givenDegreeIndexer.Append("GDG");

            return givenDegreeIndexer.ToString();
        }

        public string GenerateCourseEntityIndexer(string courseName)
        {
            StringBuilder courseIndexer = new StringBuilder();

            courseName.ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => courseIndexer.Append(i[0]));

            if (courseIndexer.Length < 5)
            {
                courseIndexer.Append("CSCSCS");
            }
            if (courseIndexer.Length > 5)
            {
                courseIndexer.Remove(5, courseIndexer.Length - 5);
            }

            var numerator = _context.courseRepository.CountCoursesWithIndexerNamePart(courseIndexer.ToString());
            courseIndexer.Append(numerator);

            if (courseIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - courseIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                courseIndexer.Insert(courseIndexer.Length - numerator.Length, fillByZero);
            }

            courseIndexer.Append("CS");

            return courseIndexer.ToString();
        }

        public string GenerateMeetingEntityIndexer(string courseIndexer)
        {
            StringBuilder meetingIndexer = new StringBuilder();

            var splittedCourseIndexer = courseIndexer.ToUpper().Split("-", 5, StringSplitOptions.RemoveEmptyEntries).ToList();
            meetingIndexer.Append(splittedCourseIndexer[0]);

            var numerator = _context.meetingRepository.CountMeetingsWithIndexerNamePart(meetingIndexer.ToString());
            meetingIndexer.Append(numerator);

            if (meetingIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - meetingIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                meetingIndexer.Insert(meetingIndexer.Length - numerator.Length, fillByZero);
            }

            meetingIndexer.Append("MG");

            return meetingIndexer.ToString();
        }

        public string GenerateExamEntityIndexer(string examName)
        {
            StringBuilder examIndexer = new StringBuilder();

            examName.ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => examIndexer.Append(i[0]));

            if (examIndexer.Length < 5)
            {
                examIndexer.Append("EX");
            }
            if (examIndexer.Length > 5)
            {
                examIndexer.Remove(5, examIndexer.Length - 5);
            }

            var numerator = _context.examRepository.CountExamsWithIndexerNamePart(examIndexer.ToString());
            examIndexer.Append(numerator);

            if (examIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - examIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                examIndexer.Insert(examIndexer.Length - numerator.Length, fillByZero);
            }

            examIndexer.Append("EX");

            return examIndexer.ToString();
        }

        public string GenerateExamTermEntityIndexer(string examIndexer)
        {
            StringBuilder examTermIndexer = new StringBuilder();

            var splittedExamIndexer = examIndexer.ToUpper().Split("-", 5, StringSplitOptions.RemoveEmptyEntries).ToList();
            examTermIndexer.Append(splittedExamIndexer[0]);

            var numerator = _context.examTermRepository.CountExamsTermsWithIndexerNamePart(examTermIndexer.ToString());
            examTermIndexer.Append(numerator);

            if (examTermIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - examTermIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                examTermIndexer.Insert(examTermIndexer.Length - numerator.Length, fillByZero);
            }

            examTermIndexer.Append("ET");

            return examTermIndexer.ToString();
        }

        public string GenerateExamResultEntityIndexer(string examIndexer)
        {
            StringBuilder examResultIndexer = new StringBuilder();

            var splittedExamIndexer = examIndexer.ToUpper().Split("-", 5, StringSplitOptions.RemoveEmptyEntries).ToList();
            examResultIndexer.Append(splittedExamIndexer[0]);

            var numerator = _context.examResultRepository.CountExamsResultsWithIndexerNamePart(examResultIndexer.ToString());
            examResultIndexer.Append(numerator);

            if (examResultIndexer.Length < 8)
            {
                StringBuilder fillByZero = new StringBuilder();

                for (int i = 0; i < 8 - examResultIndexer.Length; i++)
                {
                    fillByZero.Append(0);
                }

                examResultIndexer.Insert(examResultIndexer.Length - numerator.Length, fillByZero);
            }

            examResultIndexer.Append("ER");

            return examResultIndexer.ToString();
        }
    }
}
