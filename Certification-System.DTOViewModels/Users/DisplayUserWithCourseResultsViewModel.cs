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

        [Display(Name = "Liczba uzyskanych punktów")]
        public int PointsEarned { get; set; }

        [Display(Name = "Maksymalna liczba punktów")]
        public int MaxAmountOfPointsToEarn { get; set; }

        [Display(Name = "Procentowy wynik z egzaminu")]
        public double PercentageOfResult { get; set; }

        [Display(Name = "Egzamin zaliczony")]
        public bool ExamPassed { get; set; }

        [Display(Name = "Termin egzaminu")]
        public int ExamOrdinalNumber { get; set; }

        [Display(Name = "Ilość obecności")]
        public int QuantityOfPresenceOnMeetings { get; set; }

        [Display(Name = "Ilość spotkań")]
        public int QuantityOfMeetings { get; set; }

        [Display(Name = "Procentowa obecność na spotkaniach")]
        public double PercentageOfUserPresenceOnMeetings { get; set; }
    }
}