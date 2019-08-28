using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamTypeOfActionViewModel
    {
        public string CourseIdentificator { get; set; }
        public string ExamIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Wybrana opcja")]
        public string SelectedOption { get; set; }
        public IList<SelectListItem> AvailableOptions { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Ilość tur w ramach egzaminu")]
        public int ExamTermsQuantity { get; set; }
    }
}
