using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class ExamTermDetailsViewModel
    {
        public string ExamTermIdentificator { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Czas [dni]")]
        public DateTime DurationDays { get; set; }

        [Display(Name = "Czas [min]")]
        public DateTime DurationMinutes { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int UsersQuantity { get; set; }

        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Display(Name = "Tura egzaminu nie oceniona")]
        public bool ExamTermNotMarked { get; set; }

        [Display(Name = "Egzamin")]
        public DisplayExamWithoutCourseViewModel Exam { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Egzaminatorzy")]
        public ICollection<DisplayCrucialDataWithContactUserViewModel> Examiners { get; set; }

        [Display(Name = "Uczestnicy tury egzaminu")]
        public ICollection<DisplayUserWithExamResults> UsersWithResults { get; set; }
    }
}
