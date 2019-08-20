using Certification_System.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetForgottenPassword),
                controller: "Account",
                values: new { userIdentificator, code },
                protocol: scheme);
        }
    }
}
