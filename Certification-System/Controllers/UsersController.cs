using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo.Model;

namespace Certification_System.Controllers
{
    public class UsersController : Controller
    {
        private IDatabaseOperations _context;
        private UserManager<CertificationPlatformUser> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;

        //public UsersController()
        //{
        //    _context = new MongoOperations();
        //}

        public UsersController(UserManager<CertificationPlatformUser> userManager, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

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

                    Courses = new List<string>(),
                    Certificates = new List<string>(),
                    Degrees = new List<string>(),
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