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
                    return RedirectToAction("AnonymouslyVerificationOfUser", "CompetenceVerification", new { userIdentificator = userIdentificator });
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
        public ActionResult VerifyCertificate()
        {
            return View();
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
                    ObjectId TryObjectIdParse;
                    ObjectId.TryParse(certificateToVerify.CertificateIdentificator, out TryObjectIdParse);
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError("UserIdentificator", "Wprowadzony identyfikator ma niewłaściwy format");
                    return View(certificateToVerify);
                }

                if (_context.certificateRepository.GetCertificateById(certificateToVerify.CertificateIdentificator) != null)
                {
                    if (this.User.IsInRole("Admin"))
                    {
                        //GivenCertificateDetails
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfCertificate", "Certificates", new { givenCertificateIdentificator = certificateToVerify.CertificateIdentificator });
                    }
                }
                else
                {
                    ModelState.AddModelError("Overall", "Nie istnieje certyfikat o podanym identyfikatorze");
                    return View(certificateToVerify);
                }
            }

            return View(certificateToVerify);
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

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL});
        }

        // GET: GenerateCertificateQR
        [AllowAnonymous]
        public ActionResult GenerateCertificateQR(string certificationIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/CompetenceVerification/VerifyCertificate?CertificateIdentificator=" + $"{certificationIdentificator}";

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL });
        }
    }
}