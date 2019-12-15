using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Repository.DAL;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Certification_System.VerificationAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CompetenceVerificationApiController : ControllerBase
    {
        private readonly MongoOperations _context;
        private readonly IMapper _mapper;

        public CompetenceVerificationApiController(
                         MongoOperations context,
                         IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // VerifyUserCompetencesByQR?userIdentificator=b38ce91a-1cab-43e5-b430-0434d7a542a0
        // GET: VerifyUserCompetencesByQR
        [Route("VerifyUserCompetencesByQR")]
        [HttpGet]
        public ActionResult VerifyUserCompetencesByQR(string userIdentificator)
        {
            if (_context.userRepository.GetUserById(userIdentificator) != null)
            {
                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserDetails", "Users", new { userIdentificator = userIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfUser", "Users", new { userIdentificator = userIdentificator });
                }
            }
            else
            {
                return RedirectToAction("VerifyUser", "CompetenceVerification", new { userIdentificator = userIdentificator, userIdentificatorNotExist = true });
            }
        }

        // VerifyGivenCertificateByQR?givenCertificateIdentificator=5ce002107e5ac431745de4cd
        // GET: VerifyGivenCertificateByQR
        [Route("VerifyGivenCertificateByQR")]
        [HttpGet]
        public ActionResult VerifyGivenCertificateByQR(string givenCertificateIdentificator, bool anonymousVerification = true)
        {
            try
            {
                ObjectId.Parse(givenCertificateIdentificator);
            }
            catch (System.Exception)
            {
                return BadRequest();
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
                return BadRequest();
            }
        }

        // VerifyGivenDegreeByQR?givenDegreeIdentificator=5d4aa2399dd655477c2c8877
        // GET: VerifyGivenDegreeByQR
        [Route("VerifyGivenDegreeByQR")]
        [HttpGet]
        public ActionResult VerifyGivenDegreeByQR(VerifyGivenDegreeViewModel givenDegreeToVerify)
        {
            try
            {
                ObjectId.Parse(givenDegreeToVerify.GivenDegreeIdentificator);
            }
            catch (System.Exception)
            {
                return RedirectToAction("VerifyGivenDegree", "CompetenceVerification", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator, givenDegreeIdentificatorBadFormat = true });
            }

            if (_context.givenDegreeRepository.GetGivenDegreeById(givenDegreeToVerify.GivenDegreeIdentificator) != null)
            {
                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction("GivenDegreeDetails", "GivenDegrees", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfGivenDegree", "GivenDegrees", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
                }
            }
            else
            {
                return RedirectToAction("VerifyGivenDegree", "CompetenceVerification", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator, givenDegreeIdentificatorNotExist = true });
            }
        }
    }
}
