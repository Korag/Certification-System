using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithTermsAndLocationViewModel
    {
        public DisplayExamWithLocationViewModel Exam { get; set; }

        [Display(Name = "Tury egzaminu")]
        public ICollection<DisplayExamTermWithoutExamWithLocationViewModel> ExamTerms { get; set; }
    }
}
