using Certification_System.Entities;
using Certification_System.DTOViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo.Model;
using Certification_System.Repository.DAL;
using AutoMapper;
using Certification_System.ServicesInterfaces;

namespace Certification_System.Controllers
{
    public class UsersController : Controller
    {
        private readonly MongoOperations _context;

        private readonly UserManager<CertificationPlatformUser> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public UsersController(
            UserManager<CertificationPlatformUser> userManager,
            RoleManager<MongoRole> roleManager,
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: DisplayAllUsers
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllUsers()
        {
            var Users = _context.userRepository.GetListOfUsers();
            List<DisplayUserViewModel> usersToDisplay = new List<DisplayUserViewModel>();

            foreach (var user in Users)
            {
                DisplayUserViewModel singleUser = _mapper.Map<DisplayUserViewModel>(user);

                singleUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                usersToDisplay.Add(singleUser);
            }

            return View(usersToDisplay);
        }

        // GET: ConfirmationOfActionOnUser
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnUser(string userIdentificator, string TypeOfAction)
        {
            if (userIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var User = _context.userRepository.GetUserById(userIdentificator);

                DisplayAllUserInformationViewModel modifiedUser = _mapper.Map<DisplayAllUserInformationViewModel>(User);

                modifiedUser.CompanyRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker).Select(z=> z.CompanyName).ToList();
                modifiedUser.CompanyRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager).Select(z=> z.CompanyName).ToList();

                return View(modifiedUser);
            }

            return RedirectToAction(nameof(AddNewUser));
        }

        // GET: AddNewUser
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewUser()
        {
            AddUserViewModel newUser = new AddUserViewModel
            {
                AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList(),
                AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList()  
            };

            return View(newUser);
        }

        // POST: AddNewUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(AddUserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                CertificationPlatformUser user = _mapper.Map<CertificationPlatformUser>(newUser);
                user.Id = _keyGenerator.GenerateNewId();
                user.SecurityStamp = _keyGenerator.GenerateNewGuid();

                if (!await _roleManager.RoleExistsAsync(newUser.SelectedRole))
                {
                    await _roleManager.CreateAsync(new CertificationPlatformUserRole(newUser.SelectedRole));
                }

                var addToRole = await _userManager.AddToRoleAsync(user, newUser.SelectedRole);

                if (newUser.SelectedRole == "Company")
                {
                    if (!await _roleManager.RoleExistsAsync("Worker"))
                    {
                        await _roleManager.CreateAsync(new CertificationPlatformUserRole("Worker"));
                    }

                    var addToWorkerRoleWhenSelectedCompanyRole = await _userManager.AddToRoleAsync(user, "Worker");
                }

                var result = await _userManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ConfirmationOfActionOnUser", "Users", new { userIdentificator = user.Id, TypeOfAction = "Add" });
                }

                newUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
                newUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

                return View(newUser);
            }

            newUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
            newUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

            return View(newUser);
        }

        // GET: EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);

            EditUserViewModel userToUpdate = _mapper.Map<EditUserViewModel>(User);
            userToUpdate.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();
            userToUpdate.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();

            return View(userToUpdate);
        }

        // POST: EditUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel editedUser)
        {
            if (ModelState.IsValid)
            {
                var OriginUser = _userManager.FindByIdAsync(editedUser.UserIdentificator).Result;
                _userManager.RemoveFromRolesAsync(OriginUser, _userManager.GetRolesAsync(OriginUser).Result.ToArray()).Wait();
                _userManager.AddToRolesAsync(OriginUser, editedUser.SelectedRole).Wait();

                OriginUser = _mapper.Map<EditUserViewModel, CertificationPlatformUser>(editedUser, OriginUser);

                _context.userRepository.UpdateUser(OriginUser);

                return RedirectToAction("ConfirmationOfActionOnUser", "Users", new { userIdentificator = OriginUser.Id, TypeOfAction = "Update" });
            }

            editedUser.AvailableRoles = _context.userRepository.GetRolesAsSelectList().ToList();
            editedUser.AvailableCompanies = _context.companyRepository.GetCompaniesAsSelectList().ToList();

            return View(editedUser);
        }

        // GET: UserDetails
        [Authorize(Roles = "Admin")]
        public ActionResult UserDetails(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);
            var GivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(User.Certificates);
            var Courses = _context.courseRepository.GetCoursesById(User.Courses);
            var GivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(User.Degrees);

            var CompaniesRoleWorker = _context.companyRepository.GetCompaniesById(User.CompanyRoleWorker);
            var CompaniesRoleManager = _context.companyRepository.GetCompaniesById(User.CompanyRoleManager);

            List<Company> Companies = CompaniesRoleWorker.ToList();

            foreach (var company in CompaniesRoleManager)
            {
                if (Companies.Where(z=> z.CompanyIdentificator == company.CompanyIdentificator).Count() == 0)
                {
                    Companies.Add(company);
                }
            }

            List<DisplayCourseViewModel> ListOfCourses = new List<DisplayCourseViewModel>();

            if (Courses.Count != 0)
            {
                foreach (var course in Courses)
                {
                    DisplayCourseViewModel singleCourse = _mapper.Map<DisplayCourseViewModel>(course);
                    singleCourse.Branches = _context.branchRepository.GetBranchesById(course.Branches);

                    ListOfCourses.Add(singleCourse);
                }
            }

            List<DisplayGivenCertificateToUserViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateToUserViewModel>();

            if (GivenCertificates.Count != 0)
            {
                foreach (var givenCertificate in GivenCertificates)
                {
                    var Course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                    DisplayGivenCertificateToUserViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;
                    singleGivenCertificate.Course = courseViewModel;

                    ListOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> ListOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (GivenDegrees.Count != 0)
            {
                foreach (var givenDegree in GivenDegrees)
                {
                    var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    ListOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            //todo: translate user roles
            UserDetailsViewModel UserDetails = _mapper.Map<UserDetailsViewModel>(User);
            UserDetails.GivenCertificates = ListOfGivenCertificates;
            UserDetails.GivenDegrees = ListOfGivenDegrees;
            UserDetails.Courses = ListOfCourses;
            UserDetails.Companies = ListOfCompanies;

            return View(UserDetails);
        }


        // GET: AnonymousVerificationOfUser
        [AllowAnonymous]
        public ActionResult AnonymousVerificationOfUser(string userIdentificator)
        {
            var User = _context.userRepository.GetUserById(userIdentificator);
            var GivenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(User.Certificates);
            var GivenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(User.Degrees);

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

            List<DisplayGivenCertificateToUserWithoutCourseViewModel> ListOfGivenCertificates = new List<DisplayGivenCertificateToUserWithoutCourseViewModel>();

            if (GivenCertificates.Count != 0)
            {
                foreach (var givenCertificate in GivenCertificates)
                {
                    var Certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);

                    DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(Certificate);

                    DisplayGivenCertificateToUserWithoutCourseViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateToUserWithoutCourseViewModel>(givenCertificate);
                    singleGivenCertificate.Certificate = certificateViewModel;

                    ListOfGivenCertificates.Add(singleGivenCertificate);
                }
            }

            List<DisplayGivenDegreeToUserViewModel> ListOfGivenDegrees = new List<DisplayGivenDegreeToUserViewModel>();

            if (GivenDegrees.Count != 0)
            {
                foreach (var givenDegree in GivenDegrees)
                {
                    var Degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);

                    DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(Degree);

                    DisplayGivenDegreeToUserViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeToUserViewModel>(givenDegree);
                    singleGivenDegree.Degree = degreeViewModel;

                    ListOfGivenDegrees.Add(singleGivenDegree);
                }
            }

            List<DisplayCompanyViewModel> ListOfCompanies = _mapper.Map<List<DisplayCompanyViewModel>>(Companies);

            UserDetailsForAnonymousViewModel UserDetails = _mapper.Map<UserDetailsForAnonymousViewModel>(User);
            UserDetails.GivenCertificates = ListOfGivenCertificates;
            UserDetails.GivenDegrees = ListOfGivenDegrees;
            UserDetails.Companies = ListOfCompanies;

            return View(UserDetails);
        }
    }
}