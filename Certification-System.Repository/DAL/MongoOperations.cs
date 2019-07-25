using Certification_System.RepositoryInterfaces;

namespace Certification_System.Repository.DAL
{
    public class MongoOperations
    {
        public IBranchRepository branchRepository {get; set;}
        public ICertificateRepository certificateRepository {get; set;}
        public ICompanyRepository companyRepository {get; set;}
        public ICourseRepository courseRepository { get; set;}
        public IDegreeRepository degreeRepository { get; set;}
        public IGivenCertificateRepository givenCertificateRepository { get; set;}
        public IGivenDegreeRepository givenDegreeRepository { get; set;}
        public IInstructorRepository instructorRepository { get; set;}
        public IMeetingRepository meetingRepository { get; set;}
        public IUserRepository userRepository { get; set;}

        public MongoOperations(IBranchRepository branchRepository, ICertificateRepository certificateRepository,
           ICompanyRepository companyRepository, ICourseRepository courseRepository,
           IDegreeRepository degreeRepository, IGivenCertificateRepository givenCertificateRepository,
           IGivenDegreeRepository givenDegreeRepository, IInstructorRepository instructorRepository,
           IMeetingRepository meetingRepository, IUserRepository userRepository)
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