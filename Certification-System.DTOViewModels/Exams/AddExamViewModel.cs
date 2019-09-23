using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamViewModel
    {
        public IList<SelectListItem> AvailableCourses { get; set; }

        [Display(Name = "Kurs")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedCourse { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, ErrorMessage = "Pole \"{0}\" musi mieć długość conajmniej {2} znaków.", MinimumLength = 6)]
        [Display(Name = "Nazwa egzaminu")]
        public string Name { get; set; }

        [Display(Name = "Opis egzaminu")]
        [MaxLength(255)]
        public string Description { get; set; }

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
        [Range(1, Int32.MaxValue, ErrorMessage = "Pole \"{0}\" musi mieć minimalną wartość \"{1}\" lub więcej.")]
        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Egzamin podzielony na tury ?")]
        public bool ExamDividedToTerms { get; set; }

        public IList<SelectListItem> AvailableExaminers { get; set; }

        [Display(Name = "Egzaminatorzy")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public ICollection<string> SelectedExaminers { get; set; }

        public IList<SelectListItem> AvailableExamTypes { get; set; }

        [Display(Name = "Typ egzaminu")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string ExamType { get; set; }
    }
}
