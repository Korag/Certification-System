using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUserToExamTermViewModel
    {
        public string UserIdentificator { get; set; }
        public string ExamIdentificator { get; set; }

        public IList<SelectListItem> AvailableExamTerms { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Tury egzaminu")]
        public string SelectedExamTerm { get; set; }

        public IList<SelectListItem> AvailableExams { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Egzamin")]
        public string SelectedExams { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Użytkownik")]
        public string SelectedUser { get; set; }
    }
}
