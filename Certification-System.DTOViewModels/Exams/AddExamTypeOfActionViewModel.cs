using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamTypeOfActionViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedOption { get; set; }
        public IList<SelectListItem> AvailableOptions { get; set; }

        public int ExamTermsQuantity { get; set; }
    }
}
