using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace Certification_System.Controllers
{
    public class DocumentGeneratorController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IGeneratorQR _generatorQR;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _environment;

        public DocumentGeneratorController(
            MongoOperations context,
            IGeneratorQR generatorQR,
            IHostingEnvironment environment,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _generatorQR = generatorQR;
            _environment = environment;
        }

        // GET: GenerateUserPhysicalIdentificator
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult GenerateUserPhysicalIdentificator(string userIdentificator, byte[] userImage)
        {
            if (!string.IsNullOrWhiteSpace(userIdentificator))
            {
                string URL = Url.VerifyUserCompetencesByQRLink(userIdentificator, Request.Scheme);
                string pathToIcon = Path.Combine(_environment.WebRootPath, "Image") + $@"\logo_ziad_medium_bitmap.bmp";

                var userQRCode = _generatorQR.GenerateQRCodeFromGivenURL(URL, pathToIcon);

                var user = _context.userRepository.GetUserById(userIdentificator);

                if (user != null)
                {
                    UserIdentificatorWithQRViewModel userData = _mapper.Map<UserIdentificatorWithQRViewModel>(user);
                    userData.QRCode = userQRCode;
                    userData.UserImage = userImage;

                    return View(userData);
                }

                return RedirectToAction("BlankMenu", "Certificates");
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }
    }
}