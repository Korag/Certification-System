using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.IO;

namespace Certification_System.Controllers
{
    public class CompetenceVerificationController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public CompetenceVerificationController(IGeneratorQR generatorQR, MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _generatorQR = generatorQR;
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
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

        // GET: VerifyCertificate
        [AllowAnonymous]
        public ActionResult VerifyCertificate(string givenCertificateIdentificator, bool certificateIdentificatorNotExist, bool certificateIdentificatorBadFormat)
        {
            VerifyCertificateViewModel certificateToVerify = new VerifyCertificateViewModel
            {
                GivenCertificateIdentificator = givenCertificateIdentificator,
                CertificateIdentificatorNotExist = certificateIdentificatorNotExist,
                CertificateIdentificatorBadFormat = certificateIdentificatorBadFormat
            };

            return View(certificateToVerify);
        }

        // POST: VerifyCertificate
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyCertificate(VerifyCertificateViewModel certificateToVerify)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ObjectId.Parse(certificateToVerify.GivenCertificateIdentificator);
                }
                catch (System.Exception)
                {
                    certificateToVerify.CertificateIdentificatorBadFormat = true;
                    return View(certificateToVerify);
                }

                if (_context.givenCertificateRepository.GetGivenCertificateById(certificateToVerify.GivenCertificateIdentificator) != null)
                {
                    if (this.User.IsInRole("Admin"))
                    {
                        return RedirectToAction("GivenCertificateDetails", "Certificates", new { givenCertificateIdentificator = certificateToVerify.GivenCertificateIdentificator });
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfCertificate", "Certificates", new { givenCertificateIdentificator = certificateToVerify.GivenCertificateIdentificator });
                    }
                }
                else
                {
                    certificateToVerify.CertificateIdentificatorNotExist = true;
                    return View(certificateToVerify);
                }
            }

            return View(certificateToVerify);
        }

        // CompetenceVerification/VerifyCertificateByQR?givenCertificateIdentificator=5ce002107e5ac431745de4cd
        // POST: VerifyCertificateByQR
        [AllowAnonymous]
        public ActionResult VerifyCertificateByQR(string givenCertificateIdentificator)
        {
            try
            {
                ObjectId.Parse(givenCertificateIdentificator);
            }
            catch (System.Exception)
            {
                return RedirectToAction("VerifyCertificate", "CompetenceVerification", new { givenCertificateIdentificator = givenCertificateIdentificator, certificateIdentificatorBadFormat = true });
            }

            if (_context.givenCertificateRepository.GetGivenCertificateById(givenCertificateIdentificator) != null)
            {
                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction("GivenCertificateDetails", "Certificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfCertificate", "Certificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
                }
            }
            else
            {
                return RedirectToAction("VerifyCertificate", "CompetenceVerification", new { givenCertificateIdentificator = givenCertificateIdentificator, certificateIdentificatorNotExist = true });
            }
        }

        // GET: GenerateQRCodeFromGivenURL
        public ActionResult GenerateQRCodeFromGivenURL(string URL)
        {
            var QRBitmap = _generatorQR.GenerateQRCode(URL);

            using (MemoryStream stream = new MemoryStream())
            {
                QRBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var ByteArray = stream.ToArray();

                return File(ByteArray, "image/jpeg");
            }
        }

        // GET: GenerateUserQR
        [AllowAnonymous]
        public ActionResult GenerateUserQR(string userIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/CompetenceVerification/VerifyUserCompetencesByQR?userIdentificator=" + $"{userIdentificator}";

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL });
        }

        // GET: GenerateCertificateQR
        [AllowAnonymous]
        public ActionResult GenerateCertificateQR(string givenCertificateIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/CompetenceVerification/VerifyCertificateByQR?givenCertificateIdentificator=" + $"{givenCertificateIdentificator}";

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL });
        }
    }
}