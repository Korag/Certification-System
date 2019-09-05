using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUserToExamTermViewModel
    {
        public IList<SelectListItem> AvailableExamTerms { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Tury egzaminu")]
        public string SelectedExamTerm { get; set; }

        public string UserIdentificator { get; set; }
        public string ExamIdentificator { get; set; }
    }
}
