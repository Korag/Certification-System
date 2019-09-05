using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUserToCourseViewModel
    {
        public IList<SelectListItem> AvailableCourses { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Kursy")]
        public string SelectedCourse { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Użytkownicy")]
        public ICollection<string> SelectedUsers { get; set; }
    }
}
