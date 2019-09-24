using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithTermsViewModel
    {
        public DisplayExamViewModel Exam { get; set; }

        [Display(Name = "Tury egzaminu")]
        public ICollection<DisplayExamTermWithLocationViewModel> ExamTerms { get; set; }
    }
}
