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
        public ActionResult AddNewGivenCertificate()
        {
            AddGivenCertificateViewModel newGivenCertificate = new AddGivenCertificateViewModel
            {
                AvailableCertificates = _context.certificateRepository.GetCertificatesAsSelectList().ToList(),
                AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList(),
                AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList()
            };

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
                givenCertificate.Course = _context.courseRepository.GetCourseById(newGivenCertificate.SelectedCourses).CourseIdentificator;
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

                return RedirectToAction("ConfirmationOfActionOnGivenCertificate", "Certificates", new { givenCertificateIdentificator = OriginGivenCertificate.GivenCertificateIdentificator, TypeOfAction = "Update" });
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

            var CompaniesRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker);
            var CompaniesRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager);

            List<Company> Companies = CompaniesRoleWorker.ToList();

            foreach (var company in CompaniesRoleManager)
            {
                if (Companies.Where(z => z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    Companies.Add(company);
                }
            }

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(User);
            DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

            GivenCertificateDetailsForAnonymousViewModel VerifiedGivenCertificate = _mapper.Map<GivenCertificateDetailsForAnonymousViewModel>(GivenCertificate);
            VerifiedGivenCertificate.Certificate = certificateViewModel;
            VerifiedGivenCertificate.User = userViewModel;
            VerifiedGivenCertificate.Companies = companiesViewModel;

            return View(VerifiedGivenCertificate);
        }
    }
}