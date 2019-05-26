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

        public UsersController(UserManager<CertificationPlatformUser> userManager, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            _context = new MongoOperations();
        }

        // GET: DisplayAllUsers
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllUsers()
        {
            var Users = _context.GetUsers();
            List<DisplayUsersViewModel> usersToDisplay = new List<DisplayUsersViewModel>();

            foreach (var user in Users)
            {
                DisplayUsersViewModel singleUser = new DisplayUsersViewModel
                {
                    UserIdentificator = user.Id,

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
        public ActionResult AddNewUserConfirmation(string userIdentificator, string TypeOfAction)
        {
            if (userIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var User = _context.GetUserById(userIdentificator);

                DisplayAllUserInformationViewModel addedUser = new DisplayAllUserInformationViewModel
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

                    Roles = User.Roles
                };

                addedUser.CompanyRoleWorker = _context.GetCompaniesById(User.CompanyRoleWorker).Select(z=> z.CompanyName).ToList();
                addedUser.CompanyRoleManager = _context.GetCompaniesById(User.CompanyRoleManager).Select(z=> z.CompanyName).ToList();

                return View(addedUser);
            }
            return RedirectToAction(nameof(AddNewUser));
        }

        // GET: AddNewUser
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewUser()
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
        public async Task<ActionResult> AddNewUser(AddUserViewModel newUser)
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
                    //user.CompanyRoleWorker.Add(newUser.CompanyRoleWorker);
                    user.CompanyRoleWorker = newUser.CompanyRoleWorker;
                }

                if (newUser.CompanyRoleManager != null)
                {
                    //user.CompanyRoleManager.Add(newUser.CompanyRoleManager);
                    user.CompanyRoleManager = newUser.CompanyRoleManager;
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
                    return RedirectToAction("AddNewUserConfirmation", "Users", new { userIdentificator = user.Id, TypeOfAction = "Add" });
                }
                return View(newUser);
            }

            newUser.AvailableRoles = _context.GetRolesAsSelectList().ToList();
            newUser.AvailableCompanies = _context.GetCompaniesAsSelectList().ToList();
            return View(newUser);
        }

        // GET: EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string userIdentificator)
        {
            var User = _context.GetUserById(userIdentificator);

            EditUserViewModel userToUpdate = new EditUserViewModel
            {
                UserIdentificator = User.Id,

                Email = User.Email,
                PhoneNumber = User.PhoneNumber,

                FirstName = User.FirstName,
                LastName = User.LastName,
                DateOfBirth = User.DateOfBirth,

                Country = User.Country,
                City = User.City,
                PostCode = User.PostCode,
                Address = User.Address,
                NumberOfApartment = User.NumberOfApartment,
                
                AvailableCompanies = _context.GetCompaniesAsSelectList().ToList(),
                AvailableRoles = _context.GetRolesAsSelectList().ToList(),

                CompanyRoleManager = User.CompanyRoleManager,
                CompanyRoleWorker = User.CompanyRoleWorker,

                SelectedRole = User.Roles
            };

            return View(userToUpdate);
        }

        // POST: EditUser
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel editedUser)
        {
            if (ModelState.IsValid)
            {
                var OriginUser = _context.GetUserById(editedUser.UserIdentificator);

                _userManager.RemoveFromRolesAsync(OriginUser, OriginUser.Roles).Wait();

                OriginUser.UserName = editedUser.Email;
                OriginUser.Email = editedUser.Email;
                OriginUser.NormalizedUserName = editedUser.Email.ToUpper();
                OriginUser.NormalizedEmail = editedUser.Email.ToUpper();

                OriginUser.FirstName = editedUser.FirstName;
                OriginUser.LastName = editedUser.LastName;
                OriginUser.PhoneNumber = editedUser.PhoneNumber;
                OriginUser.DateOfBirth = editedUser.DateOfBirth;

                OriginUser.Country = editedUser.Country;
                OriginUser.City = editedUser.City;
                OriginUser.PostCode = editedUser.PostCode;
                OriginUser.Address = editedUser.Address;
                OriginUser.NumberOfApartment = editedUser.NumberOfApartment;

                OriginUser.CompanyRoleManager = editedUser.CompanyRoleManager;
                OriginUser.CompanyRoleWorker = editedUser.CompanyRoleWorker;

                _userManager.AddToRolesAsync(OriginUser, editedUser.SelectedRole).Wait();    

                _context.UpdateUser(OriginUser);

                return RedirectToAction("AddNewUserConfirmation", "Users", new { userIdentificator = editedUser.UserIdentificator, TypeOfAction = "Update" });
            }

            editedUser.AvailableRoles = _context.GetRolesAsSelectList().ToList();
            editedUser.AvailableCompanies = _context.GetCompaniesAsSelectList().ToList();
            return View(editedUser);
        }
    }
}