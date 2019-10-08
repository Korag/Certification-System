using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataExamTermViewModel
    {
        public string ExamTermIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string ExamTermIndexer { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }   
    }
}
