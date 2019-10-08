using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamResultToUserViewModel
    {
        public string ExamResultIdentificator { get; set; }

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

        public bool CanUserResignFromExam { get; set; }

        [Display(Name = "Egzamin")]
        public DisplayCrucialDataExamWithDatesViewModel Exam { get; set; }

        [Display(Name = "Tura egzaminu")]
        public DisplayCrucialDataExamTermViewModel ExamTerm { get; set; }
    }
}
