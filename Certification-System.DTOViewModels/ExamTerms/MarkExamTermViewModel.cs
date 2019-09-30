using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class MarkExamTermViewModel
    {
        [Display(Name = "Tura egzaminu")]
        public DisplayExamTermWithLocationViewModel ExamTerm { get; set; }

        [Display(Name = "Maks. liczba punktów")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Pole \"{0}\" musi mieć minimalną wartość \"{1}\" lub więcej.")]
        public double MaxAmountOfPointsToEarn { get; set; }

        [Display(Name = "Użytkownicy")]
        public MarkUserViewModel[] Users { get; set; }
    }
}
