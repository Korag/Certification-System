using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class UserDetailsViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Role użytkownika")]
        public ICollection<string> Roles { get; set; }

        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Display(Name = "Data urodzenia")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Zrzeszony z przedsiębiorstwem")]
        public ICollection<string> CompanyRoleWorker { get; set; }

        [Display(Name = "Zarządza przedsiębiorstwem")]
        public ICollection<string> CompanyRoleManager { get; set; }

        [Display(Name = "Kursy")]
        public ICollection<DisplayCourseViewModel> Courses { get; set; }

        [Display(Name = "Certyfikaty")]
        public ICollection<DisplayGivenCertificateToUserViewModel> GivenCertificates { get; set; }

        [Display(Name = "Stopnie zawodowe")]
        public ICollection<DisplayGivenDegreeToUserViewModel> GivenDegrees { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
