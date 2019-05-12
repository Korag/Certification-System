using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certification_System.Controllers
{
    public class UsersController : Controller
    {
        private IDatabaseOperations _context;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UsersController()
        {
            _context = new MongoOperations();
        }

        public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _context = new MongoOperations();
        }

        // GET: DisplayAllUsers
        [Authorize(Roles = "Admin")]
        public IActionResult DisplayAllUsers()
        {
            var Users = _context.GetUsers();
            List<DisplayUsersViewModel> usersToDisplay = new List<DisplayUsersViewModel>();

            foreach (var user in Users)
            {
                DisplayUsersViewModel singleUser = new DisplayUsersViewModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    Roles = user.Roles,

                    CompanyRoleManager = new List<string>(),
                    CompanyRoleWorker = new List<string>()
                };

                singleUser.CompanyRoleManager = _context.GetCompaniesById(user.CompanyRoleManager).Select(s => s.CompanyName).ToList();
                singleUser.CompanyRoleWorker = _context.GetCompaniesById(user.CompanyRoleWorker).Select(s => s.CompanyName).ToList();

                usersToDisplay.Add(singleUser);
            }

            return View(usersToDisplay);
        }

        // GET: AddNewUserConfirmation
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewUserConfirmation(string userIdentificator)
        {
            if (userIdentificator != null)
            {
                var User = _context.GetUserById(userIdentificator);

                AddUserViewModel addedUser = new AddUserViewModel
                {
                    Email = User.Email,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Country = User.Country,
                    City = User.City,
                    PostCode = User.PostCode,
                    Address = User.Address,
                    NumberOfApartment = User.NumberOfApartment,
                    DateOfBirth = User.DateOfBirth,
                    PhoneNumber = User.PhoneNumber,

                    SelectedRole = User.Roles.First()
                };

                addedUser.CompanyRoleWorker = _context.GetCompanyById(User.CompanyRoleWorker.First()).CompanyName;
                addedUser.CompanyRoleManager = _context.GetCompanyById(User.CompanyRoleManager.First()).CompanyName;

                return View(addedUser);
            }
            return RedirectToAction(nameof(AddNewUser));
        }

        // GET: AddNewUser
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewUser()
        {
            AddUserViewModel newUser = new AddUserViewModel
            {
                AvailableRoles = _context.GetRolesAsSelectList().ToList(),
                AvailableCompanies = _context.GetCompaniesAsSelectList().ToList()
            };

            return View(newUser);
        }

        // POST: AddNewUser
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewUser(AddUserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                var user = new CertificationPlatformUser
                {
                    Id = ObjectId.GenerateNewId().ToString(),

                    UserName = newUser.Email,
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Country = newUser.Country,
                    City = newUser.City,
                    PostCode = newUser.PostCode,
                    Address = newUser.Address,
                    NumberOfApartment = newUser.NumberOfApartment,
                    DateOfBirth = newUser.DateOfBirth,
                    PhoneNumber = newUser.PhoneNumber,

                    CompanyRoleWorker = new List<string>(),
                    CompanyRoleManager = new List<string>()
                };

                if (newUser.CompanyRoleWorker != null)
                {
                    user.CompanyRoleWorker.Add(newUser.CompanyRoleWorker);
                }

                if (newUser.CompanyRoleManager != null)
                {
                    user.CompanyRoleManager.Add(newUser.CompanyRoleManager);
                }

                user.Roles.Add(newUser.SelectedRole);

                if (newUser.SelectedRole == "Company")
                {
                    user.Roles.Add("Worker");
                }

                var result = await UserManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("AddNewUserConfirmation", "Users", new { userIdentificator = user.Id });
                }
                return View(newUser);
            }

            newUser.AvailableRoles = _context.GetRolesAsSelectList().ToList();
            newUser.AvailableCompanies = _context.GetCompaniesAsSelectList().ToList();
            return View(newUser);
        }

    }
}