using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Adres e-mail")]
        [EmailAddress(ErrorMessage = "W polu \"{0}\" nie znajduje się poprawny adres email.")]
        public string Email { get; set; }
    }
}
