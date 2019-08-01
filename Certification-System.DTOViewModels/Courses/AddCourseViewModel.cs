﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddCourseViewModel
    {
        public string CourseIdentificator { get; set; }

        [Required]
        [Display(Name = "Identyfikator")]
        public string CourseIndexer { get; set; }

        [Required]
        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Limit uczestników")]
        public int EnrolledUsersLimit { get; set; }

        public bool CourseEnded { get; set; }

        public int CourseLength { get; set; }


        [Required(ErrorMessage = "Należy zaznaczyć conajmniej jeden Obszar.")]
        public ICollection<string> SelectedBranches { get; set; }

        [Display(Name = "Obszar")]
        public IList<SelectListItem> AvailableBranches { get; set; }

        public ICollection<string> EnrolledUsers { get; set; }

        public ICollection<AddMeetingViewModel> MeetingsViewModels { get; set; }
    }
}