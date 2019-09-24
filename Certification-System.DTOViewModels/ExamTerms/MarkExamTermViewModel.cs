using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class MarkExamTermViewModel
    {
        [Display(Name = "Tura egzaminu")]
        public DisplayExamTermWithLocationViewModel ExamTerm { get; set; }

        [Display(Name = "Maks. liczba punktów")]
        public double MaxAmountOfPointsToEarn { get; set; }

        [Display(Name = "Oceniani uczestnicy")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public ICollection<MarkUserViewModel> Users { get; set; }
    }
}
