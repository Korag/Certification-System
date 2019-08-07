using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataUserViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
    }
}