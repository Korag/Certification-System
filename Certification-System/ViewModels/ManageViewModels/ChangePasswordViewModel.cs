using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Dotychczasowe hasło")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Nowe hasło musi posiadać conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Wprowadzone hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
