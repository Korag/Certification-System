using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class UserDetailsForAnonymousViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Pracownik przedsiębiorstwa")]
        public ICollection<string> CompanyRoleWorker { get; set; }

        [Display(Name = "Zarządzający przedsiębiorstwem")]
        public ICollection<string> CompanyRoleManager { get; set; }

        [Display(Name = "Certyfikaty")]
        public ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> GivenCertificates { get; set; }

        [Display(Name = "Certyfikaty")]
        public ICollection<DisplayGivenDegreeToUserViewModel> GivenDegrees { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}