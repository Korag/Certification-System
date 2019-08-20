using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Certification_System.DTOViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Certification_System.Entities;
using Certification_System.ServicesInterfaces;
using Certification_System.Extensions;
using AspNetCore.Identity.Mongo.Model;
using AutoMapper;
using Certification_System.Repository.DAL;
using Certification_System.DTOViewModels;
using System.Collections.Generic;

namespace Certification_System.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly MongoOperations _context;

        private readonly UserManager<CertificationPlatformUser> _userManager;
        private readonly SignInManager<CertificationPlatformUser> _signInManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;

        public AccountController(
            MongoOperations context,
            UserManager<CertificationPlatformUser> userManager,
            SignInManager<CertificationPlatformUser> signInManager,
            RoleManager<MongoRole> roleManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IMapper mapper,
            IKeyGenerator keyGenerator)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        private readonly Dictionary<int, string> _messages = new Dictionary<int, string>
         {
           {1,"Na podany przez Ciebie email została wysłana wiadomość"},
           {2, "Two"},
           {3,"Three"}
         };


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl = null, string message = null)
        {
            ViewBag.message = message;

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Adres email lub hasło są błędne.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<CertificationPlatformUser>(model);
                user.Id = _keyGenerator.GenerateNewId();
                user.SecurityStamp = _keyGenerator.GenerateNewGuid();

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!await _roleManager.RoleExistsAsync("Worker"))
                {
                    await _roleManager.CreateAsync(new CertificationPlatformUserRole("Worker"));
                }

                var addToRole = await _userManager.AddToRoleAsync(user, "Worker");

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, callbackUrl, "");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangeAccountPassword(string userIdentificator)
        {
            ChangePasswordViewModel passwordToChange = new ChangePasswordViewModel
            {
                UserIdentificator = userIdentificator
            };

            return View(passwordToChange);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangeAccountPassword(ChangePasswordViewModel passwordToChange)
        {
            if (ModelState.IsValid)
            {
                var User = _context.userRepository.GetUserById(passwordToChange.UserIdentificator);

                var result = _userManager.ChangePasswordAsync(User, passwordToChange.OldPassword, passwordToChange.Password).Result;

                if (result.Succeeded)
                {
                    _signInManager.SignOutAsync().Wait();
                    return RedirectToAction("Login", "Account", new { message = "Twoje hasło zostało zmienione - zostałeś wylogowany" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wprowadzono niepoprawne obecnie obowiązujące hasło");
                    return View(new ChangePasswordViewModel { UserIdentificator = passwordToChange.UserIdentificator });
                }
            }

            return View(new ChangePasswordViewModel { UserIdentificator = passwordToChange.UserIdentificator });
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(Login), "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // todo: link setAccountPassword to login action in if clause
        // todo: link emailVerification to login action in if clause and ModelState.AddError

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SetAccountPassword(string userId)
        {
            SetPasswordViewModel setPassword = new SetPasswordViewModel
            {
                UserIdentificator = userId
            };

            return View(setPassword);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetAccountPassword(SetPasswordViewModel passwordToSet)
        {
            if (ModelState.IsValid)
            {
                var user = _context.userRepository.GetUserById(passwordToSet.UserIdentificator);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { message = "Wystąpił niespodziewany błąd podczas ustawiania hasła użytkownika" });

                }

                var addPasswordResult = _userManager.AddPasswordAsync(user, passwordToSet.Password).Result;

                if (addPasswordResult.Succeeded)
                {
                    _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("BlankMenu", "Certificates", new { message = "Twoje hasło zostało ustawione - zostałeś zalogowany na swoje konto" });
                }
            }

            return View(passwordToSet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel emailModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(emailModel.Email);

                //todo: after creating EmailVerificationMethod

                if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // generate URL and send email with Code

                //var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                //await _emailSender.SendEmailAsync(emailModel.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(emailModel);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(int messageNumber)
        {
            ViewBag.message = _messages[messageNumber];

            return View();
        }


        #region Currently not-used

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new CertificationPlatformUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(CertificatesController.BlankMenu), "Certificates");
            }
        }

        #endregion
    }
}
