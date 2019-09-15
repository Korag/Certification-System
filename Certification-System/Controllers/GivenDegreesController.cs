using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.Services;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class GivenDegreesController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;

        public GivenDegreesController(
            MongoOperations context, 
            IGeneratorQR generatorQR, 
            IMapper mapper, 
            IKeyGenerator keyGenerator,
            ILogService logger)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
        }

        // GET: EditGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult EditGivenDegree(string givenDegreeIdentificator)
        {
            var GivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);
            EditGivenDegreeViewModel givenDegreeToUpdate = _mapper.Map<EditGivenDegreeViewModel>(GivenDegree);

            givenDegreeToUpdate.User = _mapper.Map<DisplayCrucialDataUserViewModel>(_context.userRepository.GetUserByGivenDegreeId(GivenDegree.GivenDegreeIdentificator));
            givenDegreeToUpdate.Degree = _mapper.Map<DisplayCrucialDataDegreeViewModel>(_context.degreeRepository.GetDegreeById(GivenDegree.Degree));

            return View(givenDegreeToUpdate);
        }

        // POST: EditGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGivenDegree(EditGivenDegreeViewModel editedGivenDegree)
        {
            if (ModelState.IsValid)
            {
                var OriginGivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(editedGivenDegree.GivenDegreeIdentificator);

                OriginGivenDegree = _mapper.Map<EditGivenDegreeViewModel, GivenDegree>(editedGivenDegree, OriginGivenDegree);
                _context.givenDegreeRepository.UpdateGivenDegree(OriginGivenDegree);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, LogTypeOfAction.TypesOfActions[1]);
                _logger.AddGivenDegreeLog(OriginGivenDegree, logInfo);

                return RedirectToAction("ConfirmationOfActionOnGivenDegree", "GivenDegrees", new { givenDegreeIdentificator = OriginGivenDegree.GivenDegreeIdentificator, TypeOfAction = "Update" });
            }

            return View(editedGivenDegree);
        }

        // GET: ConfirmationOfActionOnGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnGivenDegree(string givenDegreeIdentificator, string TypeOfAction)
        {
            if (givenDegreeIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var GivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

                var Degree = _context.degreeRepository.GetDegreeById(GivenDegree.Degree);
                var User = _context.userRepository.GetUserByGivenDegreeId(GivenDegree.GivenDegreeIdentificator);

                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(User);
                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);

                DisplayGivenDegreeViewModel modifiedGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(GivenDegree);
                modifiedGivenDegree.Degree = degreeViewModel;
                modifiedGivenDegree.User = userViewModel;

                return View(modifiedGivenDegree);
            }

            return RedirectToAction(nameof(AddNewGivenDegree));
        }

        // GET: DisplayAllGivenDegrees
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllGivenDegrees()
        {
            var GivenDegrees = _context.givenDegreeRepository.GetListOfGivenDegrees();
            List<DisplayGivenDegreeViewModel> ListOfGivenDegrees = new List<DisplayGivenDegreeViewModel>();

            foreach (var givenDegree in GivenDegrees)
            {
                var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                var User = _context.userRepository.GetUserByGivenDegreeId(givenDegree.GivenDegreeIdentificator);

                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(User);

                DisplayGivenDegreeViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(givenDegree);
                singleGivenDegree.Degree = degreeViewModel;
                singleGivenDegree.User = userViewModel;

                ListOfGivenDegrees.Add(singleGivenDegree);
            }

            return View(ListOfGivenDegrees);
        }

        // GET: AddNewGivenDegree
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewGivenDegree(string userIdentificator)
        {
            AddGivenDegreeViewModel newGivenDegree = new AddGivenDegreeViewModel
            {
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                newGivenDegree.SelectedUser = userIdentificator;
            }

            return View(newGivenDegree);
        }

        // POST: AddNewGivenDegree
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewGivenDegree(AddGivenDegreeViewModel newGivenDegree)
        {
            if (ModelState.IsValid)
            {
                GivenDegree givenDegree = _mapper.Map<GivenDegree>(newGivenDegree);
                givenDegree.GivenDegreeIdentificator = _keyGenerator.GenerateNewId();

                _context.givenDegreeRepository.AddGivenDegree(givenDegree);
                _context.userRepository.AddUserDegree(newGivenDegree.SelectedUser, givenDegree.GivenDegreeIdentificator);

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, LogTypeOfAction.TypesOfActions[0]);
                _logger.AddGivenDegreeLog(givenDegree, logInfoAdd);

                var user = _context.userRepository.GetUserById(newGivenDegree.SelectedUser);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, LogTypeOfAction.TypesOfActions[0]);
                _logger.AddUserLog(user, logInfoUpdate);

                return RedirectToAction("ConfirmationOfActionOnGivenDegree", new { givenDegreeIdentificator = givenDegree.GivenDegreeIdentificator, TypeOfAction = "Add" });
            }

            newGivenDegree.AvailableDegrees = _context.degreeRepository.GetDegreesAsSelectList().ToList();
            newGivenDegree.AvailableUsers = _context.userRepository.GetUsersAsSelectList().ToList();

            return View(newGivenDegree);
        }

        // GET: GivenDegreeDetails
        [Authorize(Roles = "Admin")]
        public ActionResult GivenDegreeDetails(string givenDegreeIdentificator)
        {
            var GivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

            var Degree = _context.degreeRepository.GetDegreeById(GivenDegree.Degree);

            var RequiredDegrees = _context.degreeRepository.GetDegreesById(Degree.RequiredDegrees);
            var RequiredCertificates = _context.certificateRepository.GetCertificatesById(Degree.RequiredCertificates);

            var User = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
            var Companies = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager.Concat(User.CompanyRoleWorker).Distinct().ToList());

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(Degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(Degree.Branches);

            DisplayAllUserInformationViewModel userViewModel = _mapper.Map<DisplayAllUserInformationViewModel>(User);

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);


            List<DisplayGivenCertificateToUserWithoutCourseViewModel> ListOfRequiredCertificatesWithInstances = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();
            var UsersGivenCertificate = _context.givenCertificateRepository.GetGivenCertificatesById(User.GivenCertificates);

            if (RequiredCertificates.Count != 0)
            {
                foreach (var certificate in RequiredCertificates)
                {
                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);

                    var RequiredGivenCertificate = UsersGivenCertificate.Where(z => z.Certificate == certificate.CertificateIdentificator).FirstOrDefault();

                    DisplayGivenCertificateToUserWithoutCourseViewModel requiredCertificateWithInstance = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(RequiredGivenCertificate);
                    requiredCertificateWithInstance.Certificate = certificateViewModel;

                    ListOfRequiredCertificatesWithInstances.Add(requiredCertificateWithInstance);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> ListOfRequiredDegreesWithInstances = new List<DisplayGivenDegreeToUserViewModel>();
            var UsersGivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(User.GivenDegrees);

            if (RequiredDegrees.Count != 0)
            {
                foreach (var degree in RequiredDegrees)
                {
                    DisplayCrucialDataDegreeViewModel singleDegreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);

                    var RequiredGivenDegree = UsersGivenDegrees.Where(z => z.Degree == degree.DegreeIdentificator).FirstOrDefault();

                    DisplayGivenDegreeToUserViewModel requiredDegreeWithInstance = _mapper.Map<DisplayGivenDegreeToUserViewModel>(singleDegreeViewModel);
                    requiredDegreeWithInstance.Degree = singleDegreeViewModel;

                    ListOfRequiredDegreesWithInstances.Add(requiredDegreeWithInstance);
                }
            }

            GivenDegreeDetailsViewModel GivenDegreeDetails = new GivenDegreeDetailsViewModel();
            GivenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(GivenDegree);
            GivenDegreeDetails.GivenDegree.Degree = degreeViewModel;
            GivenDegreeDetails.User = userViewModel;
            GivenDegreeDetails.Companies = companiesViewModel;

            GivenDegreeDetails.RequiredCertificatesWithGivenInstances = ListOfRequiredCertificatesWithInstances;
            GivenDegreeDetails.RequiredDegreesWithGivenInstances = ListOfRequiredDegreesWithInstances;

            return View(GivenDegreeDetails);
        }
        
        // GET: AnonymouslyVerificationOfGivenDegree
        [AllowAnonymous]
        public ActionResult AnonymouslyVerificationOfGivenDegree(string givenDegreeIdentificator)
        {
            var GivenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

            var Degree = _context.degreeRepository.GetDegreeById(GivenDegree.Degree);

            var User = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
            var Companies = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager.Concat(User.CompanyRoleWorker).Distinct().ToList());

            DisplayDegreeWithoutRequirementsViewModel degreeViewModel = _mapper.Map<DisplayDegreeWithoutRequirementsViewModel>(Degree);
            degreeViewModel.Branches = _context.branchRepository.GetBranchesById(Degree.Branches);

            DisplayCrucialDataWithBirthDateUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataWithBirthDateUserViewModel>(User);

            List<DisplayCompanyViewModel> companiesViewModel = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            GivenDegreeDetailsForAnonymousViewModel GivenDegreeDetails = new GivenDegreeDetailsForAnonymousViewModel();
            GivenDegreeDetails.GivenDegree = _mapper.Map<DisplayGivenDegreeToUserExtendedViewModel>(GivenDegree);
            GivenDegreeDetails.GivenDegree.Degree = degreeViewModel;
            GivenDegreeDetails.User = userViewModel;
            GivenDegreeDetails.Companies = companiesViewModel;

            return View(GivenDegreeDetails);
        }
    }
}