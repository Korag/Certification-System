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
        public readonly IGivenCertificateRepository givenCertificateRepository;
        public readonly IGivenDegreeRepository givenDegreeRepository;
        public readonly IInstructorRepository instructorRepository;
        public readonly IMeetingRepository meetingRepository;
        public readonly IUserRepository userRepository;

        public MongoOperations(
           IBranchRepository branchRepository,
           ICertificateRepository certificateRepository,
           ICompanyRepository companyRepository,
           ICourseRepository courseRepository,
           IDegreeRepository degreeRepository,
           IGivenCertificateRepository givenCertificateRepository,
           IGivenDegreeRepository givenDegreeRepository,
           IInstructorRepository instructorRepository,
           IMeetingRepository meetingRepository,
           IUserRepository userRepository)
        {
            this.branchRepository = branchRepository;
            this.certificateRepository = certificateRepository;
            this.companyRepository = companyRepository;
            this.courseRepository = courseRepository;
            this.degreeRepository = degreeRepository;
            this.givenCertificateRepository = givenCertificateRepository;
            this.givenDegreeRepository = givenDegreeRepository;
            this.instructorRepository = instructorRepository;
            this.meetingRepository = meetingRepository;
            this.userRepository = userRepository;
        }
    }
}