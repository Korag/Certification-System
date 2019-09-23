using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamPeriodViewModel
    {
        public IList<SelectListItem> AvailableCourses { get; set; }

        [Display(Name = "Kurs")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedCourse { get; set; }

        public IList<SelectListItem> AvailableExams { get; set; }

        [Display(Name = "Egzamin")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedExam{ get; set; }

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

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa kraju powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa miasta powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa ulicy powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Numer domu/mieszkania nie powinien być większy niż 10 znaków")]
        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }
    }
}
