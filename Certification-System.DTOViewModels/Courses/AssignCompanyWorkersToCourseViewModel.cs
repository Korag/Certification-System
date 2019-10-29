using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignCompanyWorkersToCourseViewModel
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string CourseIndexer { get; set; }

        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Limit uczestników")]
        public int EnrolledUsersLimit { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int EnrolledUsersQuantity { get; set; }

        [Display(Name = "Kończy się egzaminem")]
        public bool ExamIsRequired { get; set; }

        [Display(Name = "Kurs zakończony")]
        public bool CourseEnded { get; set; }

        [Display(Name = "Długość kursu [dni]")]
        public int CourseLength { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<string> Branches { get; set; }

        [Display(Name = "Liczba wolnych miejsc")]
        public int VacantSeats { get; set; }

        [Display(Name = "Lista użytkowników możliwych do dodania do egzaminu")]
        public AddUsersFromCheckBoxViewModel[] CompanyWorkersToAssignToExam { get; set; }

        [Display(Name = "Niezapisani na kurs pracownicy przedsiębiorstwa")]
        public ICollection<DisplayCrucialDataUserViewModel> CompanyWorkers { get; set; }
    }
}
