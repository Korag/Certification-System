﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerExamDetailsViewModel
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
        public int UsersQuantity { get; set; }

        [Display(Name = "Limit uczestników")]
        public int UsersLimit { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Tury egzaminu")]
        public ICollection<DisplayExamTermWithoutExamViewModel> ExamTerms { get; set; }

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
