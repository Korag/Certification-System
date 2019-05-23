using Certification_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class CourseDetailsViewModel
    {
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

        [Display(Name = "Kurs zakończony")]
        public bool CourseEnded { get; set; }

        [Display(Name = "Długość kursu [dni]")]
        public int CourseLength { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<string> SelectedBranches { get; set; }

        [Display(Name = "Spotkania w ramach szkolenia")]
        public ICollection<DisplayMeetingViewModel> Meetings { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<DisplayUsersViewModel> EnrolledUsers { get; set; }
    }
}
