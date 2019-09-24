﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DeleteUsersFromExamTermViewModel
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

        [Display(Name = "Egzamin")]
        public DisplayCrucialDataExamViewModel Exam { get; set; }

        [Display(Name = "Lista użytkowników do usunięcia z tury egzaminu")]
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromExamTerm { get; set; }

        [Display(Name = "Zapisani na turę egzaminu")]
        public ICollection<DisplayCrucialDataUserViewModel> AllExamTermParticipants { get; set; }

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
    }
}
