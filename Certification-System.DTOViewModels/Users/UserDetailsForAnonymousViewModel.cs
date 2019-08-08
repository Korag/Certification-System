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
        public string DateOfBirth { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Certyfikaty")]
        public ICollection<DisplayGivenCertificateToUserViewModel> GivenCertificates { get; set; }
    }
}