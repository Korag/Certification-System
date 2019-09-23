using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignUsersFromCourseToExamViewModel
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
        public int DurationMinutes { get; set; }

        [Display(Name = "Egzamin podzielony na tury ?")]
        public bool ExamDividedToTerms { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int UsersQuantitiy { get; set; }

        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Display(Name = "Liczba wolnych miejsc")]
        public int VacantSeats { get; set; }

        [Display(Name = "Lista użytkowników możliwych do dodania do egzaminu")]
        public AddUsersFromCheckBoxViewModel[] UsersToAssignToExam { get; set; }

        [Display(Name = "Niezapisani na egzamin uczestnicy kursu")]
        public ICollection<DisplayCrucialDataUserViewModel> CourseParticipants { get; set; }
    }
}
