using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUserToExamViewModel
    {
        public IList<SelectListItem> AvailableExams { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Egzaminy")]
        public string SelectedExam { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Użytkownik")]
        public string SelectedUser { get; set; }
    }
}
