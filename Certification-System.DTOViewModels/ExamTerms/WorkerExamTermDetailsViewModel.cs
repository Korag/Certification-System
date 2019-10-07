using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerExamTermDetailsViewModel
    {
        public string ExamTermIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string ExamTermIndexer { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Czas [dni]")]
        public int DurationDays { get; set; }

        [Display(Name = "Czas [min]")]
        public int DurationMinutes { get; set; }

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

        [Display(Name = "Wynik tury egzaminu")]
        public DisplayExamResultToUserViewModel ExamResult { get; set; }
        
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        public bool CanUserAssignToExam { get; set; }
        public bool CanUserResignFromExam { get; set; }
    }
}
