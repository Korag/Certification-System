using Certification_System.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userIdentificator, code },
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

        public static string SetUserPasswordLink(this IUrlHelper urlHelper, string userIdentificator, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.SetAccountPassword),
                controller: "Account",
                values: new { userIdentificator },
                protocol: scheme);
        }

        public static string VerifyUserCompetencesByQRLink(this IUrlHelper urlHelper, string userIdentificator, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CompetenceVerificationController.VerifyUserCompetencesByQR),
                controller: "CompetenceVerification",
                values: new { userIdentificator },
                protocol: scheme);
        }

        public static string VerifyGivenCertificateByQRLink(this IUrlHelper urlHelper, string givenCertificateIdentificator, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CompetenceVerificationController.VerifyGivenCertificateByQR),
                controller: "CompetenceVerification",
                values: new { givenCertificateIdentificator },
                protocol: scheme);
        }

        public static string VerifyGivenDegreeByQRLink(this IUrlHelper urlHelper, string givenDegreeIdentificator, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CompetenceVerificationController.VerifyGivenDegreeByQR),
                controller: "CompetenceVerification",
                values: new { givenDegreeIdentificator },
                protocol: scheme);
        }
    }
}
