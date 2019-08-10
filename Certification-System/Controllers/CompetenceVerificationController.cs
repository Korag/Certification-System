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
                        return RedirectToAction("GivenCertificateDetails", "Certificates", new { givenCertificateIdentificator = givenCertificateToVerify.GivenCertificateIdentificator });
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfGivenCertificate", "Certificates", new { givenCertificateIdentificator = givenCertificateToVerify.GivenCertificateIdentificator });
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
                    return RedirectToAction("GivenCertificateDetails", "Certificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfGivenCertificate", "Certificates", new { givenCertificateIdentificator = givenCertificateIdentificator });
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
                        return RedirectToAction("GivenDegreeDetails", "GivenDegree", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
                    }
                    else
                    {
                        return RedirectToAction("AnonymouslyVerificationOfGivenDegree", "GivenDegree", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
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
                return RedirectToAction("VerifyGivenDegree", "CompetenceVerification", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator, givenDegreeIdentificatorBadForma = true });
            }

            if (_context.givenDegreeRepository.GetGivenDegreeById(givenDegreeToVerify.GivenDegreeIdentificator) != null)
            {
                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction("GivenDegreeDetails", "GivenDegree", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
                }
                else
                {
                    return RedirectToAction("AnonymouslyVerificationOfGivenDegree", "GivenDegree", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator });
                }
            }
            else
            {
                return RedirectToAction("VerifyGivenDegree", "CompetenceVerification", new { givenDegreeIdentificator = givenDegreeToVerify.GivenDegreeIdentificator, givenDegreeIdentificatorNotExist = true });
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

        // GET: GenerateGivenCertificateQR
        [AllowAnonymous]
        public ActionResult GenerateGivenCertificateQR(string givenCertificateIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/CompetenceVerification/VerifyGivenCertificateByQR?givenCertificateIdentificator=" + $"{givenCertificateIdentificator}";

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL });
        }

        // GET: GenerateGivenDegreeQR
        [AllowAnonymous]
        public ActionResult GenerateGivenDegreeQR(string givenDegreeIdentificator)
        {
            string URL = @"https://certification-system.azurewebsites.net/CompetenceVerification/VerifyGivenDegreeByQR?givenDegreeIdentificator=" + $"{givenDegreeIdentificator}";

            return RedirectToAction("GenerateQRCodeFromGivenURL", "CompetenceVerification", new { URL = URL });
        }
    }
}