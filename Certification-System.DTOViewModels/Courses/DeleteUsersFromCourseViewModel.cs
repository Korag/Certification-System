﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DeleteUsersFromCourseViewModel
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

        [Display(Name = "Długość kursu [dni]")]
        public int CourseLength { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data otrzymania certyfikatu")]
        public DateTime ReceiptDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data wygaśnięcia certyfikatu")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<string> Branches { get; set; }

        [Display(Name = "Lista użytkowników do usunięcia z kursu")]
        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromCourse { get; set; }

        [Display(Name = "Uczestnicy kursu")]
        public ICollection<DisplayCrucialDataUserViewModel> AllCourseParticipants { get; set; }
    }
}
