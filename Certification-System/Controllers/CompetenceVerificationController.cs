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
        public ActionResult VerifyUser()
        {
            return View();
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
                    ModelState.AddModelError("Overall", "Nie istnieje użytkownik o podanym identyfikatorze");
                    return View(userToVerify);
                }
            }

            return View(userToVerify);
        }

        // GET: VerifyUserCompetencesByQR
        //[AllowAnonymous]
        //public ActionResult VerifyUserCompetencesByQR(string userIdentificator)
        //{
        //    if (this.User.IsInRole("Admin"))
        //    {
        //        return RedirectToAction("UserDetails", "Users", new { userIdentificator = userIdentificator });
        //    }
        //    else
        //    {
        //        return RedirectToAction("AnonymouslyVerificationOfUser", "Users", new { userIdentificator = userIdentificator });
        //    }
        //}

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

        // GET: GenerateUserQR
        [AllowAnonymous]
        public ActionResult GenerateUserQR(string userIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/Certificates/VerifyUser?UserIdentificator=" + $"{userIdentificator}";
            var QRBitmap = _generatorQR.GenerateQRCode(URL);

            using (MemoryStream stream = new MemoryStream())
            {
                QRBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var ByteArray = stream.ToArray();

                return File(ByteArray, "image/jpeg");
            }
        }
    }
}