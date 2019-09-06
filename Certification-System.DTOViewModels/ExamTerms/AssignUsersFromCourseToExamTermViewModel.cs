using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUsersFromCourseToExamTermViewModel
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

        [Display(Name = "Liczba wolnych miejsc")]
        public int VacantSeats { get; set; }

        [Display(Name = "Egzamin")]
        public DisplayCrucialDataExamViewModel Exam { get; set; }

        [Display(Name = "Lista użytkowników możliwych do dodania do egzaminu")]
        public AddUsersFromCheckBoxViewModel[] UsersToAssignToExamTerm { get; set; }

        [Display(Name = "Niezapisani na egzamin uczestnicy kursu")]
        public ICollection<DisplayCrucialDataUserViewModel> CourseParticipants { get; set; }
    }
}
