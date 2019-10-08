using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataExamWithDatesViewModel
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
    }
}
