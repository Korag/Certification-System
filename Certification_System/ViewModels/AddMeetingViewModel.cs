﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Certification_System.ViewModels
{
    public class AddMeetingViewModel
    {
        [Required(ErrorMessage = "Należy wybrać do którego kursu przypisać spotkanie.")]
        public string SelectedCourse { get; set; }

        [Display(Name = "Kurs")]
        public IList<SelectListItem> AvailableCourses { get; set; }

        [Required]
        [Display(Name = "Identyfikator")]
        public string MeetingIndexer { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Data szkolenia")]
        public DateTime DateOfMeeting { get; set; }

        [Required]
        [Display(Name = "Państwo")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Required]
        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Display(Name = "Instruktor")]
        public ICollection<string> Instructors { get; set; }

        [Display(Name = "Lista Obecności")]
        public ICollection<string> AttendanceList { get; set; }
    }
}