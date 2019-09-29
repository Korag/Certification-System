using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamResultWithExamIdentificator
    {
        [Display(Name = "Identyfikator wyniku")]
        public string ExamResultIndexer { get; set; }

        [Display(Name = "Ilość punktów")]
        public string PointsEarned { get; set; }

        [Display(Name = "Maks. liczba punktów")]
        public double MaxAmountOfPointsToEarn { get; set; }

        [Display(Name = "Wynik procentowy")]
        public double PercentageOfResult { get; set; }

        [Display(Name = "Egzamin zaliczony")]
        public bool ExamPassed { get; set; }

        [Display(Name = "Identyfikator egzaminu")]
        public string ExamIdentificator { get; set; }

        [Display(Name = "Termin egzaminu")]
        public int ExamOrdinalNumber { get; set; }
    }
}
