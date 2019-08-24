using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamTermWithoutExamViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }


        public IList<SelectListItem> AvailableExaminers { get; set; }

        [Display(Name = "Egzaminatorzy")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public ICollection<string> SelectedExaminers { get; set; }
    }
}
