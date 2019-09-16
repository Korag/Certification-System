using Certification_System.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string BlankMenuLink(this IUrlHelper urlHelper, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CertificatesController.BlankMenu),
                controller: "Certificates",
                values: new { },
                protocol: scheme);
        }

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

        public static string DeleteBranchEntityLink(this IUrlHelper urlHelper, string branchIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(BranchesController.DeleteBranch),
                controller: "Branches",
                values: new { branchIdentificator, code },
                protocol: scheme);
        }

        public static string DeleteCertificateEntityLink(this IUrlHelper urlHelper, string certificateIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CertificatesController.DeleteCertificate),
                controller: "Certificates",
                values: new { certificateIdentificator, code },
                protocol: scheme);
        }

        public static string DeleteCompanyEntityLink(this IUrlHelper urlHelper, string companyIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(CompaniesController.DeleteCompany),
                controller: "Companies",
                values: new { companyIdentificator, code },
                protocol: scheme);
        }

        public static string DeleteGivenDegreeEntityLink(this IUrlHelper urlHelper, string givenDegreeIdentificator, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(GivenDegreesController.DeleteGivenDegree),
                controller: "GivenDegrees",
                values: new { givenDegreeIdentificator, code },
                protocol: scheme);
        }
    }
}
