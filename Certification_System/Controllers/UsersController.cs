using Certification_System.DAL;
using Certification_System.Models;
using Certification_System.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult DisplayAllUsers()
        {
            return View();
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
                var user = new ApplicationUser
                {
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

                var result = await UserManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("AddNewUserConfirmation", "Users");
                }
                return View(newUser);
            }

            newUser.AvailableRoles = _context.GetRolesAsSelectList().ToList();
            newUser.AvailableCompanies = _context.GetCompaniesAsSelectList().ToList();
            return View(newUser);
        }

    }
}