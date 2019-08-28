using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddCourseTypeOfActionViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Ilość spotkań w ramach kursu")]
        public int MeetingsQuantity { get; set; }
    }
}
