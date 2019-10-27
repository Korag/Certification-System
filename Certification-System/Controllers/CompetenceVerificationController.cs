using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Certification_System.Controllers
{
    public class CompetenceVerificationController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IHostingEnvironment _environment;

        public CompetenceVerificationController(
                         MongoOperations context,
                         IGeneratorQR generatorQR,
                         IMapper mapper,
                         IKeyGenerator keyGenerator,
                         IHostingEnvironment environment)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _environment = environment;
        }

        // GET: VerifyCompetenceManual
        [AllowAnonymous]
        public ActionResult VerifyCompetenceManual()
        {
            return View();
        }

        // GET: VerifyUser
        [AllowAnonymous]
        public ActionResult VerifyUser(string userIdentificator, bool userIdentificatorNotExist)
        {
            VerifyUserViewModel userToVerify = new VerifyUserViewModel
            {
                UserIdentificator = userIdentificator,
                UserIdentificatorNotExist = userIdentificatorNotExist
            };

            return View(userToVerify);
        }

        // CompetenceVerification/VerifyUserCompetencesByQR?userIdentificator=b38ce91a-1cab-43e5-b430-0434d7a542a0
        // GET: VerifyUserCompetencesByQR
        [AllowAnonymous]
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

        // POST: VerifyUser
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyUser(VerifyUserViewModel userToVerify)
        {
            if (ModelState.IsValid)
            {
                if (_context.userRepository.GetUserById(userToVerify.UserIdentificator) != null)
                {
                    if (this.User.IsInRole("Admin"))
                    {
                        return RedirectToAction("UserDetails", "Users", new { userIdentificator = userToVerify.UserIdentificator });
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfUser", "Users", new { userIdentificator = userToVerify.UserIdentificator });
                    }
                }
                else
                {
                    userToVerify.UserIdentificatorNotExist = true;
                    return View(userToVerify);
                }
            }

            return View(userToVerify);
        }

        // GET: VerifyGivenCertificate
        [AllowAnonymous]
        public ActionResult VerifyGivenCertificate(string givenCertificateIdentificator, bool givenCertificateIdentificatorNotExist, bool givenCertificateIdentificatorBadFormat)
        {
            VerifyGivenCertificateViewModel givenCertificateToVerify = new VerifyGivenCertificateViewModel
            {
                GivenCertificateIdentificator = givenCertificateIdentificator,
                GivenCertificateIdentificatorNotExist = givenCertificateIdentificatorNotExist,
                GivenCertificateIdentificatorBadFormat = givenCertificateIdentificatorBadFormat
            };

            return View(givenCertificateToVerify);
        }

        // POST: VerifyGivenCertificate
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyGivenCertificate(VerifyGivenCertificateViewModel givenCertificateToVerify)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ObjectId.Parse(givenCertificateToVerify.GivenCertificateIdentificator);
                }
                catch (System.Exception)
                {
                    givenCertificateToVerify.GivenCertificateIdentificatorBadFormat = true;
                    return View(givenCertificateToVerify);
                }

                if (_context.givenCertificateRepository.GetGivenCertificateById(givenCertificateToVerify.GivenCertificateIdentificator) != null)
                {
                    if (this.User.IsInRole("Admin"))
                    {
                        return RedirectToAction("GivenCertificateDetails", "GivenCertificates", new { givenCertificateIdentificator = givenCertificateToVerify.GivenCertificateIdentificator });
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfGivenCertificate", "GivenCertificates", new { givenCertificateIdentificator = givenCertificateToVerify.GivenCertificateIdentificator });
                    }
                }
                else
                {
                    givenCertificateToVerify.GivenCertificateIdentificatorNotExist = true;
                    return View(givenCertificateToVerify);
                }
            }

            return View(givenCertificateToVerify);
        }

        // CompetenceVerification/VerifyGivenCertificateByQR?givenCertificateIdentificator=5ce002107e5ac431745de4cd
        // GET: VerifyGivenCertificateByQR
        [AllowAnonymous]
        public ActionResult VerifyGivenCertificateByQR(string givenCertificateIdentificator)
        {
            try
            {
                ObjectId.Parse(givenCertificateIdentificator);
            }
            catch (System.Exception)
            {
                return RedirectToAction("VerifyGivenCertificate", "CompetenceVerification", new { givenCertificateIdentificator = givenCertificateIdentificator, givenCertificateIdentificatorBadFormat = true });
            }

            if (_context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator) != null)
            {
                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction("GivenCertificateDetails", "GivenCertificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfGivenCertificate", "GivenCertificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
                }
            }
            else
            {
                return RedirectToAction("VerifyGivenCertificate", "CompetenceVerification", new { givenCertificateIdentificator = givenCertificateIdentificator, givenCertificateIdentificatorNotExist = true });
            }
        }

        // GET: VerifyGivenDegree
        [AllowAnonymous]
        public ActionResult VerifyGivenDegree(string givenDegreeIdentificator, bool givenDegreeIdentificatorNotExist, bool givenDegreeIdentificatorBadFormat)
        {
            VerifyGivenDegreeViewModel givenDegreeToVerify = new VerifyGivenDegreeViewModel
            {
                GivenDegreeIdentificator = givenDegreeIdentificator,
                GivenDegreeIdentificatorNotExist = givenDegreeIdentificatorNotExist,
                GivenDegreeIdentificatorBadFormat = givenDegreeIdentificatorBadFormat
            };

            return View(givenDegreeToVerify);
        }

        // POST: VerifyGivenDegree
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyGivenDegree(VerifyGivenDegreeViewModel givenDegreeToVerify)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ObjectId.Parse(givenDegreeToVerify.GivenDegreeIdentificator);
                }
                catch (System.Exception)
                {
                    givenDegreeToVerify.GivenDegreeIdentificatorBadFormat = true;
                    return View(givenDegreeToVerify);
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
                    givenDegreeToVerify.GivenDegreeIdentificatorNotExist = true;
                    return View(givenDegreeToVerify);
                }
            }

            return View(givenDegreeToVerify);
        }

        // CompetenceVerification/VerifyGivenDegreeByQR?givenDegreeIdentificator=5d4aa2399dd655477c2c8877
        // GET: VerifyGivenDegreeByQR
        [AllowAnonymous]
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

        // GET: CompanyUsersGivenCompetences
        [Authorize(Roles = "Company")]
        public ActionResult CompanyUsersGivenCompetences()
        {
            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());

            var givenCertificates = _context.givenCertificateRepository.GetGivenCertificatesById(companyWorkers.SelectMany(z=> z.GivenCertificates).ToList());
            var givenDegrees = _context.givenDegreeRepository.GetGivenDegreesById(companyWorkers.SelectMany(z => z.GivenDegrees).ToList());

            List<DisplayGivenCertificateViewModel> listOfGivenCertificates = new List<DisplayGivenCertificateViewModel>();

            foreach (var givenCertificate in givenCertificates)
            {
                var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                var user = companyWorkers.Where(z => z.GivenCertificates.Contains(givenCertificate.GivenCertificateIdentificator)).FirstOrDefault();

                DisplayCrucialDataCourseViewModel courseViewModel = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                DisplayCrucialDataCertificateViewModel certificateViewModel = _mapper.Map<DisplayCrucialDataCertificateViewModel>(certificate);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);

                DisplayGivenCertificateViewModel singleGivenCertificate = _mapper.Map<DisplayGivenCertificateViewModel>(givenCertificate);
                singleGivenCertificate.Certificate = certificateViewModel;
                singleGivenCertificate.Course = courseViewModel;
                singleGivenCertificate.User = userViewModel;

                listOfGivenCertificates.Add(singleGivenCertificate);
            }

            List<DisplayGivenDegreeViewModel> listOfGivenDegrees = new List<DisplayGivenDegreeViewModel>();

            foreach (var givenDegree in givenDegrees)
            {
                var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                var user = companyWorkers.Where(z => z.GivenDegrees.Contains(givenDegree.GivenDegreeIdentificator)).FirstOrDefault();

                DisplayCrucialDataDegreeViewModel degreeViewModel = _mapper.Map<DisplayCrucialDataDegreeViewModel>(degree);
                DisplayCrucialDataUserViewModel userViewModel = _mapper.Map<DisplayCrucialDataUserViewModel>(user);

                DisplayGivenDegreeViewModel singleGivenDegree = _mapper.Map<DisplayGivenDegreeViewModel>(givenDegree);
                singleGivenDegree.Degree = degreeViewModel;
                singleGivenDegree.User = userViewModel;

                listOfGivenDegrees.Add(singleGivenDegree);
            }

            DisplayCompanyWorkersGivenCompetencesViewModel companyUsersCompetences = new DisplayCompanyWorkersGivenCompetencesViewModel
            {
                CompanyIdentificator = companyManager.CompanyRoleManager.FirstOrDefault(),
                
                GivenCertificates = listOfGivenCertificates,
                GivenDegrees = listOfGivenDegrees
            };

            return View(companyUsersCompetences);
        }

        // GET: GenerateUserPhysicalIdentificator
        [Authorize(Roles = "Admin, Worker, Company")]
        public ActionResult GenerateUserPhysicalIdentificator(string userIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                var user = _context.userRepository.GetUserById(userIdentificator);

                if (user != null)
                {
                    GetUserImageViewModel userImage = _mapper.Map<GetUserImageViewModel>(user);

                    return View("GenerateUserPhysicalIdentificatorPreparation", userImage);
                }

                return RedirectToAction("BlankMenu", "Certificates");
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        //https://localhost:44378/CompetenceVerification/GenerateUserPhysicalIdentificator?userIdentificator=5d6ff3d85596d1c6a9e44124
        // GET: GenerateUserPhysicalIdentificator
        [Authorize(Roles = "Admin, Worker, Company")]
        [HttpPost]
        public ActionResult GenerateUserPhysicalIdentificator(GetUserImageViewModel userImage)
        {
            if (!string.IsNullOrWhiteSpace(userImage.UserIdentificator))
            {
                var user = _context.userRepository.GetUserById(userImage.UserIdentificator);

                if (user != null)
                {
                    string URL = Url.VerifyUserCompetencesByQRLink(userImage.UserIdentificator, Request.Scheme);
                    string pathToIcon = Path.Combine(_environment.WebRootPath, "Image") + $@"\logo_ziad_medium_bitmap.bmp";

                    var userQRCode = _generatorQR.GenerateQRCodeFromGivenURL(URL, pathToIcon);

                    UserIdentificatorWithQRViewModel userData = _mapper.Map<UserIdentificatorWithQRViewModel>(user);
                    userData.QRCode = userQRCode;

                    if (userImage.Image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            userImage.Image.CopyToAsync(memoryStream);
                            userData.UserImage = memoryStream.ToArray();
                        }
                    }

                    return View(userData);
                }

                return RedirectToAction("BlankMenu", "Certificates");
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: GenerateGivenCertificatePossessionConfirmation
        [Authorize(Roles = "Admin, Worker, Company")]
        public ActionResult GenerateGivenCertificatePossessionConfirmation(string givenCertificateIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(givenCertificateIdentificator))
            {
                var givenCertificate = _context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator);

                if (givenCertificate != null)
                {
                    string URL = Url.VerifyGivenCertificateByQRLink(givenCertificateIdentificator, Request.Scheme);
                    string pathToIcon = Path.Combine(_environment.WebRootPath, "Image") + $@"\logo_ziad_medium_bitmap.bmp";

                    var userQRCode = _generatorQR.GenerateQRCodeFromGivenURL(URL, pathToIcon);

                    var user = _context.userRepository.GetUserByGivenCertificateId(givenCertificateIdentificator);
                    var certificate = _context.certificateRepository.GetCertificateById(givenCertificate.Certificate);
                    var branches = _context.branchRepository.GetBranchesById(certificate.Branches);
                    var course = _context.courseRepository.GetCourseById(givenCertificate.Course);
                    var exams = _context.examRepository.GetExamsById(course.Exams).Where(z=> z.OrdinalNumber == 1);

                    UserGivenCertificatePossessionConfirmationViewModel userData = _mapper.Map<UserGivenCertificatePossessionConfirmationViewModel>(user);
                    userData.QRCode = userQRCode;
                    userData.Branches = branches;

                    userData = _mapper.Map<GivenCertificate, UserGivenCertificatePossessionConfirmationViewModel>(givenCertificate, userData);
                    userData = _mapper.Map<Certificate, UserGivenCertificatePossessionConfirmationViewModel>(certificate, userData);
                    userData = _mapper.Map<Course, UserGivenCertificatePossessionConfirmationViewModel>(course, userData);

                    userData.Exams = _mapper.Map<List<DisplayExamNameTypeViewModel>>(exams);

                    return View(userData);
                }

                return RedirectToAction("BlankMenu", "Certificates");
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: GenerateGivenDegreePossessionConfirmation
        [Authorize(Roles = "Admin, Worker, Company")]
        public ActionResult GenerateGivenDegreePossessionConfirmation(string givenDegreeIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(givenDegreeIdentificator))
            {
                var givenDegree = _context.givenDegreeRepository.GetGivenDegreeById(givenDegreeIdentificator);

                if (givenDegree != null)
                {
                    string URL = Url.VerifyGivenDegreeByQRLink(givenDegreeIdentificator, Request.Scheme);
                    string pathToIcon = Path.Combine(_environment.WebRootPath, "Image") + $@"\logo_ziad_medium_bitmap.bmp";

                    var userQRCode = _generatorQR.GenerateQRCodeFromGivenURL(URL, pathToIcon);

                    var user = _context.userRepository.GetUserByGivenDegreeId(givenDegreeIdentificator);
                    var degree = _context.degreeRepository.GetDegreeById(givenDegree.Degree);
                    var branches = _context.branchRepository.GetBranchesById(degree.Branches);

                    var requiredCertificatesNames = _context.certificateRepository.GetCertificatesById(degree.RequiredCertificates).Select(z => z.Name).ToList();
                    var requiredDegreesNames = _context.degreeRepository.GetDegreesById(degree.RequiredDegrees).Select(z => z.Name).ToList();

                    UserGivenDegreePossessionConfirmationViewModel userData = _mapper.Map<UserGivenDegreePossessionConfirmationViewModel>(user);
                    userData.QRCode = userQRCode;
                    userData.Branches = branches;

                    userData = _mapper.Map<GivenDegree, UserGivenDegreePossessionConfirmationViewModel>(givenDegree, userData);
                    userData = _mapper.Map<Degree, UserGivenDegreePossessionConfirmationViewModel>(degree, userData);

                    userData.RequiredCertificatesNames = requiredCertificatesNames;
                    userData.RequiredDegreesNames = requiredDegreesNames;

                    return View(userData);
                }

                return RedirectToAction("BlankMenu", "Certificates");
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }
    }
}

