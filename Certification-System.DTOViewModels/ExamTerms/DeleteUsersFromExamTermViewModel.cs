using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DeleteUsersFromExamTermViewModel
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

        [Display(Name = "Egzamin")]
        public DisplayCrucialDataExamViewModel Exam { get; set; }

        [Display(Name = "Lista użytkowników do usunięcia z tury egzaminu")]
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromExamTerm { get; set; }

        [Display(Name = "Zapisani na turę egzaminu")]
        public ICollection<DisplayCrucialDataUserViewModel> AllExamTermParticipants { get; set; }
    }
}
