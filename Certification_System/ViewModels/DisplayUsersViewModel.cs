using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class DisplayUsersViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Imię powinno zawierać od 3 do 100 znaków.")]
        [DataType(DataType.Text)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwisko powinno składać się od 3 do 100 znaków")]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Role użytkownika")]
        [Required(ErrorMessage = "Należy przydzielić rolę")]
        public ICollection<string> Roles { get; set; }


        [Display(Name = "Zrzeszony z przedsiębiorstwem")]
        public ICollection<string> CompanyRoleWorker { get; set; }

        [Display(Name = "Zarządza przedsiębiorstwem")]
        public ICollection<string> CompanyRoleManager { get; set; }
    }
}