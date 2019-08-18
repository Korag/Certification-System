using Certification_System.RepositoryInterfaces;

namespace Certification_System.Repository.DAL
{
    public class MongoOperations
    {
        public readonly IBranchRepository branchRepository;
        public readonly ICertificateRepository certificateRepository;
        public readonly ICompanyRepository companyRepository;
        public readonly ICourseRepository courseRepository;
        public readonly IDegreeRepository degreeRepository;
        public readonly IExamRepository examRepository;
        public readonly IGivenCertificateRepository givenCertificateRepository;
        public readonly IGivenDegreeRepository givenDegreeRepository;
        public readonly IMeetingRepository meetingRepository;
        public readonly IUserRepository userRepository;

        public MongoOperations(
           IBranchRepository branchRepository,
           ICertificateRepository certificateRepository,
           ICompanyRepository companyRepository,
           ICourseRepository courseRepository,
           IDegreeRepository degreeRepository,
           IExamRepository examRepository,
           IGivenCertificateRepository givenCertificateRepository,
           IGivenDegreeRepository givenDegreeRepository,
           IMeetingRepository meetingRepository,
           IUserRepository userRepository)
        {
            this.branchRepository = branchRepository;
            this.certificateRepository = certificateRepository;
            this.companyRepository = companyRepository;
            this.courseRepository = courseRepository;
            this.degreeRepository = degreeRepository;
            this.examRepository = examRepository;
            this.givenCertificateRepository = givenCertificateRepository;
            this.givenDegreeRepository = givenDegreeRepository;
            this.meetingRepository = meetingRepository;
            this.userRepository = userRepository;
        }
    }
}