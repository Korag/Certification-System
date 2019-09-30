using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamWithoutCourseViewModel
    {
        public string ExamIdentificator { get; set; }

        [Display(Name = "Identyfikator egzaminu")]
        public string ExamIndexer { get; set; }

        [Display(Name = "Termin")]
        public string OrdinalNumber { get; set; }

        [Display(Name = "Nazwa egzaminu")]
        public string Name { get; set; }

        [Display(Name = "Opis egzaminu")]
        public string Description { get; set; }

        [Display(Name = "Typ egzaminu")]
        public string TypeOfExam { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Czas [dni]")]
        public int DurationDays { get; set; }

        [Display(Name = "Czas [min]")]
        public int DurationMinutes{ get; set; }

        [Display(Name = "Egzamin podzielony na tury ?")]
        public bool ExamDividedToTerms { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int UsersQuantity { get; set; }

        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Display(Name = "Egzaminatorzy")]
        public ICollection<DisplayCrucialDataUserViewModel> Examiners { get; set; }
    }
}
