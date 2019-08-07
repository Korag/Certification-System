using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataWithCompaniesRoleUserViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Zrzeszony z przedsiębiorstwem")]
        public ICollection<string> CompanyRoleWorker { get; set; }

        [Display(Name = "Zarządza przedsiębiorstwem")]
        public ICollection<string> CompanyRoleManager { get; set; }
    }
}