using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithExamResults
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Identyfikator wyniku")]
        public string ExamResultIndexer { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Został oceniony")]
        public bool HasExamResult { get; set; }

        [Display(Name = "Liczba uzyskanych punktów")]
        public double PointsEarned { get; set; }

        [Display(Name = "Maksymalna liczba punktów")]
        public double MaxAmountOfPointsToEarn { get; set; }

        [Display(Name = "Procentowy wynik z egzaminu")]
        public double PercentageOfResult { get; set; }

        [Display(Name = "Egzamin zaliczony")]
        public bool ExamPassed { get; set; }
    }
}