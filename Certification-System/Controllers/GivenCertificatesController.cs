using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class GivenCertificatesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public GivenCertificatesController(IGeneratorQR generatorQR, MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: DisplayAllGivenCertificates
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenCertificates()
        {
            var GivenCertificates = _context.givenCertificateRepository.GetListOfGivenCertificates();
            List<DisplayGivenCertificateViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateViewModel>();

            foreach (var givenCertificate in GivenCertificates)
            {
                var Course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var User = _context.userRepository.GetUserByGivenCertificateId(givenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(User);

                DisplayGivenCertificateViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(givenCertificate);
                singleGivenCertificate.Certificate = certificateViewModel;
                singleGivenCertificate.Course = courseViewModel;
                singleGivenCertificate.User = userViewModel;

                ListOfGivenCertificates.Add(singleGivenCertificate);
            }

            return View(ListOfGivenCertificates);
        }

        // GET: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenCertificate(string userIdentificator)
        {
            AddGivenCertificateViewModel newGivenCertificate = new AddGivenCertificateViewModel
            {
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableCourses = _context.courseRepository.GetAllCoursesAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                var user = _context.userRepository.GetUserById(userIdentificator);

                newGivenCertificate.AvailableCourses = _context.courseRepository.GenerateSelectList(user.Courses).ToList();
            }

            return View(newGivenCertificate);
        }

        // POST: AddNewGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewGivenCertificate(AddGivenCertificateViewModel newGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                GivenCertificate givenCertificate = _mapper.Map<GivenCertificate>(newGivenCertificate);
                givenCertificate.GivenCertificateIdentificator = _keyGenerator.GenerateNewId();
                givenCertificate.Course = _context.courseRepository.GetCourseById(newGivenCertificate.SelectedCourse).CourseIdentificator;
                givenCertificate.Certificate = _context.certificateRepository.GetCertificateById(newGivenCertificate.SelectedCertificate).CertificateIdentificator;

                _context.givenCertificateRepository.AddGivenCertificate(givenCertificate);
                _context.userRepository.AddUserCertificate(newGivenCertificate.SelectedUser, givenCertificate.GivenCertificateIdentificator);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", new { givenCertificateIdentificator = givenCertificate.GivenCertificateIdentificator, TypeOfAction = "Add" });
            }

            newGivenCertificate.AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList();
            newGivenCertificate.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();
            newGivenCertificate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(newGivenCertificate);
        }

        // GET: ConfirmationOfActionOnGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnGivenCertificate(string givenCertificateIdentificator, string TypeOfAction)
        {
            if (givenCertificateIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

                var Course = _context.courseRepository.GetCourseById(GivenCertificate.Course);
                var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);
                var User = _context.userRepository.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);

                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(User);
                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                DisplayGivenCertificateViewModel modifiedGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(GivenCertificate);
                modifiedGivenCertificate.Course = courseViewModel;
                modifiedGivenCertificate.Certificate = certificateViewModel;
                modifiedGivenCertificate.User = userViewModel;

                return View(modifiedGivenCertificate);
            }

            return RedirectToAction(nameof(AddNewGivenCertificate));
        }

        // GET: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        public ActionResult EditGivenCertificate(string givenCertificateIdentificator)
        {
            var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var OwnerOfTheCertificate = _context.userRepository.GetUserByGivenCertificateId(givenCertificateIdentificator);
            var CourseWhichEndedWithSuchCertificate = _context.courseRepository.GetCourseById(GivenCertificate.Course);
            var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);

            EditGivenCertificateViewModel givenCertificateToUpdate = _mapper.Map<EditGivenCertificateViewModel>(GivenCertificate);
            givenCertificateToUpdate.User = _mapper.Map<DisplayCrucialDataUserViewModel>(OwnerOfTheCertificate);
            givenCertificateToUpdate.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(CourseWhichEndedWithSuchCertificate);
            givenCertificateToUpdate.Certificate = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

            return View(givenCertificateToUpdate);
        }

        // POST: EditGivenCertificate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenCertificate(EditGivenCertificateViewModel editedGivenCertificate)
        {
            if (ModelState.IsValid)
            {
                var OriginGivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(editedGivenCertificate.GivenCertificateIdentificator);

                OriginGivenCertificate = _mapper.Map<EditGivenCertificateViewModel, GivenCertificate>(editedGivenCertificate, OriginGivenCertificate);
                _context.givenCertificateRepository.UpdateGivenCertificate(OriginGivenCertificate);

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", "GivenCertificates", new { givenCertificateIdentificator = OriginGivenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenCertificate);
        }

        // GET: AnonymouslyVerificationOfGivenCertificate
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfGivenCertificate(string givenCertificateIdentificator)
        {
            var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);

            var User = _context.userRepository.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);
            var Companies = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager.Concat(User.CompanyRoleWorker).Distinct().ToList());

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(User);

            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

            GivenCertificateDetailsForAnonymousViewModel VerifiedGivenCertificate = new GivenCertificateDetailsForAnonymousViewModel();

            VerifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(GivenCertificate);
            VerifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;
            VerifiedGivenCertificate.User = userViewModel;
            VerifiedGivenCertificate.Companies = companiesViewModel;

            return View(VerifiedGivenCertificate);
        }

        // GET: GivenCertificateDetails
        [Authorize(Roles = "Admin")]
        public ActionResult GivenCertificateDetails(string givenCertificateIdentificator)
        {
            var GivenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);
            var Certificate = _context.certificateRepository.GetCertificateById(GivenCertificate.Certificate);

            var User = _context.userRepository.GetUserByGivenCertificateId(GivenCertificate.GivenCertificateIdentificator);
            var Companies = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager.Concat(User.CompanyRoleWorker).Distinct().ToList());

            var Course = _context.courseRepository.GetCourseById(GivenCertificate.Course);
            var Meetings = _context.meetingRepository.GetMeetingsById(Course.Meetings);

            List<string> InstructorsIdentificators = new List<string>();
            Meetings.ToList().ForEach(z=> z.Instructors.ToList().ForEach(s=> InstructorsIdentificators.Add(s)));
            InstructorsIdentificators.Distinct();

            var Instructors = _context.userRepository.GetInstructorsById(InstructorsIdentificators);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(Course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(courseViewModel.Branches);

            List<DisplayMeetingWithoutCourseViewModel> meetingsViewModel = new List<DisplayMeetingWithoutCourseViewModel>();

            foreach (var meeting in Meetings)
            {
                DisplayMeetingWithoutCourseViewModel singleMeeting =  _mapper.Map<DisplayMeetingWithoutCourseViewModel>(meeting);
                singleMeeting.Instructors = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Instructors.Where(z => meeting.Instructors.Contains(z.Id)).ToList());

                meetingsViewModel.Add(singleMeeting);
            }

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(User);
            List<DisplayCrucialDataWithContactUserViewModel> instructorsViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Instructors);

            DisplayCertificateViewModel certificateViewModel = _mapper.Map<DisplayCertificateViewModel>(Certificate);
            certificateViewModel.Branches = _context.branchRepository.GetBranchesById(certificateViewModel.Branches);

            GivenCertificateDetailsViewModel VerifiedGivenCertificate = new GivenCertificateDetailsViewModel();
            VerifiedGivenCertificate.GivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseExtendedViewModel>(GivenCertificate);
            VerifiedGivenCertificate.GivenCertificate.Certificate = certificateViewModel;

            VerifiedGivenCertificate.Course = courseViewModel;
            VerifiedGivenCertificate.Meetings = meetingsViewModel;

            VerifiedGivenCertificate.User = userViewModel;
            VerifiedGivenCertificate.Instructors = instructorsViewModel;
            VerifiedGivenCertificate.Companies = companiesViewModel;

            return View(VerifiedGivenCertificate);
        }
    }
}