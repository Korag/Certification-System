using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DeleteEntityViewModel
    {
        public string EntityIdentificator { get; set; }

        public string Code { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, ErrorMessage = "Pole \"{0}\" musi posiadać conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Pole \"{0}\" musi posiadać conajmniej {2} znaków.", MinimumLength = 6)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Wprowadzone hasła różnią się.")]
        public string ConfirmPassword { get; set; }
    }
}
