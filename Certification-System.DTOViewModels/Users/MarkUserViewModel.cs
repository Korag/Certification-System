using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class MarkUserViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Liczba uzyskanych punktów")]
        public double PointsEarned { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Procentowy wynik z egzaminu")]
        public double PercentageOfResult { get; set; }

        [Display(Name = "Egzamin zaliczony")]
        public bool ExamPassed { get; set; }
    }
}