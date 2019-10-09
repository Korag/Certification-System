using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class SelfAssignUserToExamViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Egzamin")]
        public string SelectedExam { get; set; }

        public IList<SelectListItem> AvailableExamsPeriods { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Termin egzaminu")]
        public string SelectedExamPeriod { get; set; }

        public IList<SelectListItem> AvailableExamsTerms { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Tura egzaminu")]
        public string SelectedExamTerm { get; set; }

        public string CourseIdentificator { get; set; }
        public string ExamIndexer { get; set; }
    }
}
