using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string UserIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string Code { get; set; }
    }
}
