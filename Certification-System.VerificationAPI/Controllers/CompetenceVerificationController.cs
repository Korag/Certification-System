using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Certification_System.VerificationAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CompetenceVerificationController : ControllerBase
    {
        private readonly MongoOperations _context;
        private readonly IMapper _mapper;

        public CompetenceVerificationController(
                         MongoOperations context,
                         IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: VerifyUserCompetences
        [Route("VerifyUserCompetences")]
        [HttpGet]
        public ActionResult VerifyUserCompetences(string userIdentificator, bool anonymousVerification = true)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            if (user != null)
            {
                var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);
                var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

                var companiesRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker);
                var companiesRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager);

                List<Company> companies = companiesRoleWorker.ToList();

                foreach (var company in companiesRoleManager)
                {
                    if (companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                    {
                        companies.Add(company);
                    }
                }

                if (anonymousVerification)
                {
                    List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();

                    if (givenCertificates.Count != 0)
                    {
                        foreach (var givenCertificate in givenCertificates)
                        {
                            var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                            DisplayGivenCertificateToUserWithoutCourseViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
                            singleGivenCertificate.Certificate = certificateViewModel;

                            listOfGivenCertificates.Add(singleGivenCertificate);
                        }
                    }

                    List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

                    if (givenDegrees.Count != 0)
                    {
                        foreach (var givenDegree in givenDegrees)
                        {
                            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                            DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                            DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                            singleGivenDegree.Degree = degreeViewModel;

                            listOfGivenDegrees.Add(singleGivenDegree);
                        }
                    }

                    List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

                    UserDetailsForAnonymousViewModel verifiedUser = _mapper.Map<UserDetailsForAnonymousViewModel>(user);
                    verifiedUser.GivenCertificates = listOfGivenCertificates;
                    verifiedUser.GivenDegrees = listOfGivenDegrees;
                    verifiedUser.Companies = ListOfCompanies;

                    return Ok(verifiedUser);
                }
                else
                {
                    var courses = _context.courseRepository.GetCoursesById(user.Courses);

                    List<DisplayCourseViewModel> listOfCourses = new List<DisplayCourseViewModel>();

                    if (courses.Count != 0)
                    {
                        foreach (var course in courses)
                        {
                            DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                            singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                            listOfCourses.Add(singleCourse);
                        }
                    }

                    List<DisplayGivenCertificateToUserViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

                    if (givenCertificates.Count != 0)
                    {
                        foreach (var givenCertificate in givenCertificates)
                        {
                            var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                            var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                            DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                            DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                            singleGivenCertificate.Certificate = certificateViewModel;
                            singleGivenCertificate.Course = courseViewModel;

                            listOfGivenCertificates.Add(singleGivenCertificate);
                        }
                    }

                    List<DisplayGivenDegreeToUserViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

                    if (givenDegrees.Count != 0)
                    {
                        foreach (var givenDegree in givenDegrees)
                        {
                            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                            DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                            DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                            singleGivenDegree.Degree = degreeViewModel;

                            listOfGivenDegrees.Add(singleGivenDegree);
                        }
                    }

                    List<DisplayCompanyViewModel> listOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

                    UserDetailsViewModel verifiedUser = _mapper.Map<UserDetailsViewModel>(user);
                    verifiedUser.Roles = _context.userRepository.TranslateRoles(verifiedUser.Roles);

                    verifiedUser.GivenCertificates = listOfGivenCertificates;
                    verifiedUser.GivenDegrees = listOfGivenDegrees;
                    verifiedUser.Courses = listOfCourses;
                    verifiedUser.Companies = listOfCompanies;

                    return Ok(verifiedUser);
                }
            }
            else
            {
                return BadRequest("Użytkownik nie został odnaleziony");
            }
        }

        // GET: VerifyGivenCertificate
        [Route("VerifyGivenCertificate")]
        [HttpGet]
        public ActionResult VerifyGivenCertificate(string givenCertificateIdentificator, bool anonymousVerification = true)
        {
            try
            {
                ObjectId.Parse(givenCertificateIdentificator);
            }
            catch (System.Exception)
            {
                return BadRequest("Błędny format identyfikatora");
            }

            var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

            if (givenCertificate != null)
            {
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);
                var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

                if (anonymousVerification)
                {
                    List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);
                    DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(user);
                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);
                    GivenCertificateDetailsForAnonymousViewModel verifiedGivenCertificate = new GivenCertificateDetailsForAnonymousViewModel();

                    verifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
                    verifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;
                    verifiedGivenCertificate.User = userViewModel;
                    verifiedGivenCertificate.Companies = companiesViewModel;

                    return Ok(verifiedGivenCertificate);
                }
                else
                {
                    var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var meetings = _context.meetingRepository.GetMeetingsById(course.Meetings);

                    List<string> instructorsIdentificators = new List<string>();
                    meetings.ToList().ForEach(z => z.Instructors.ToList().ForEach(s => instructorsIdentificators.Add(s)));
                    instructorsIdentificators.Distinct();

                    var instructors = _context.userRepository.GetInstructorsById(instructorsIdentificators);

                    DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
                    courseViewModel.Branches = _context.branchRepository.GetBranchesById(courseViewModel.Branches);

                    List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

                    foreach (var meeting in meetings)
                    {
                        DisplayMeetingWithoutCourseViewModel singleMeeting = _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                        singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(instructors.Where(z => meeting.Instructors.Contains(z.Id)).ToList());

                        meetingsViewModel.Add(singleMeeting);
                    }

                    List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

                    DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(user);
                    userViewModel.Roles = _context.userRepository.TranslateRoles(userViewModel.Roles);

                    List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(instructors);

                    DisplayCertificateViewModel certificateViewModel = _mapper.Map<DisplayCertificateViewModel>(certificate);
                    certificateViewModel.Branches = _context.branchRepository.GetBranchesById(certificateViewModel.Branches);

                    GivenCertificateDetailsViewModel verifiedGivenCertificate = new GivenCertificateDetailsViewModel();
                    verifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseExtendedViewModel>(givenCertificate);
                    verifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;

                    verifiedGivenCertificate.Course = courseViewModel;
                    verifiedGivenCertificate.Meetings = meetingsViewModel;

                    verifiedGivenCertificate.User = userViewModel;
                    verifiedGivenCertificate.Instructors = instructorsViewModel;
                    verifiedGivenCertificate.Companies = companiesViewModel;

                    return Ok(verifiedGivenCertificate);
                }
            }
            else
            {
                return BadRequest("Certyfikat nie został odnaleziony");
            }
        }

        // GET: VerifyGivenDegree
        [Route("VerifyGivenDegree")]
        [HttpGet]
        public ActionResult VerifyGivenDegree(string givenDegreeIdentificator, bool anonymousVerification = true)
        {
            try
            {
                ObjectId.Parse(givenDegreeIdentificator);
            }
            catch (System.Exception)
            {
                return BadRequest("Błędny format identyfikatora");
            }

            var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);
            var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
            var user = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
            var companies = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager.Concat(user.CompanyRoleWorker).Distinct().ToList());

            if (givenDegree != null)
            {
                if (anonymousVerification)
                {
                    DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
                    degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

                    DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(user);

                    List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

                    GivenDegreeDetailsForAnonymousViewModel verifiedGivenDegree = new GivenDegreeDetailsForAnonymousViewModel();
                    verifiedGivenDegree.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
                    verifiedGivenDegree.GivenDegree.Degree = degreeViewModel;
                    verifiedGivenDegree.User = userViewModel;
                    verifiedGivenDegree.Companies = companiesViewModel;

                    return Ok(verifiedGivenDegree);
                }
                else
                {
                    var requiredDegrees = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees);
                    var requiredCertificates = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates);

                    DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(degree);
                    degreeViewModel.Branches = _context.branchRepository.GetBranchesById(degree.Branches);

                    DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(user);

                    List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(companies);

                    List<DisplayGivenCertificateToUserWithoutCourseViewModel> listOfRequiredCertificatesWithInstances = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();
                    var UsersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(user.GivenCertificates);

                    if (requiredCertificates.Count != 0)
                    {
                        foreach (var certificate in requiredCertificates)
                        {
                            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                            var requiredGivenCertificate = UsersGivenCertificate.Where(z => z.Certificate == certificate.CertificateIdentificator).FirstOrDefault();

                            DisplayGivenCertificateToUserWithoutCourseViewModel requiredCertificateWithInstance = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(requiredGivenCertificate);
                            requiredCertificateWithInstance.Certificate = certificateViewModel;

                            listOfRequiredCertificatesWithInstances.Add(requiredCertificateWithInstance);
                        }
                    }

                    List<DisplayGivenDegreeToUserViewModel> listOfRequiredDegreesWithInstances = new List<DisplayGivenDegreeToUserViewModel>();
                    var usersGivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(user.GivenDegrees);

                    if (requiredDegrees.Count != 0)
                    {
                        foreach (var requiredSingleDegree in requiredDegrees)
                        {
                            DisplayCrucialDataDegreeViewModel singleDegreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(requiredSingleDegree);

                            var requiredGivenDegree = usersGivenDegrees.Where(z => z.Degree == degree.DegreeIdentificator).FirstOrDefault();

                            DisplayGivenDegreeToUserViewModel requiredDegreeWithInstance = _mapper.Map<DisplayGivenDegreeToUserViewModel>(singleDegreeViewModel);
                            requiredDegreeWithInstance.Degree = singleDegreeViewModel;

                            listOfRequiredDegreesWithInstances.Add(requiredDegreeWithInstance);
                        }
                    }

                    GivenDegreeDetailsViewModel verifiedGivenDegree = new GivenDegreeDetailsViewModel();
                    verifiedGivenDegree.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(givenDegree);
                    verifiedGivenDegree.GivenDegree.Degree = degreeViewModel;
                    verifiedGivenDegree.User = userViewModel;
                    verifiedGivenDegree.Companies = companiesViewModel;

                    verifiedGivenDegree.RequiredCertificatesWithGivenInstances = listOfRequiredCertificatesWithInstances;
                    verifiedGivenDegree.RequiredDegreesWithGivenInstances = listOfRequiredDegreesWithInstances;

                    return Ok(verifiedGivenDegree);
                }
            }
            else
            {
                return BadRequest("Stopień zawodowy nie został odnaleziony");
            }
        }
    }
}
