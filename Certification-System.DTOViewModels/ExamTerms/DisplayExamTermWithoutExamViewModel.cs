using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamTermWithoutExamViewModel
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
        public int UsersQuantitiy { get; set; }

        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Display(Name = "Egzaminatorzy")]
        public ICollection<DisplayCrucialDataUserViewModel> Examiners { get; set; }
    }
}
