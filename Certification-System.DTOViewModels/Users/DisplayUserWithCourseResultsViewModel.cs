using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithCourseResultsViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Procentowy wynik z egzaminu")]
        public double ExamPercentResult { get; set; }

        [Display(Name = "Liczba podejść do egzaminu")]
        public double ExamAttempsQuantity { get; set; }

        [Display(Name = "Procentowa obecność na spotkaniach")]
        public double PercentageOfUserPresenceOnMeetings { get; set; }
    }
}