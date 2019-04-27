using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Certification_System.ViewModels
{
    public class AddCourseViewModel
    {
        [Required]
        [Display(Name = "Identyfikator")]
        public string CourseIdentificator { get; set; }

        [Required]
        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        public int CourseLength { get; set; }


        [Required(ErrorMessage = "Należy zaznaczyć conajmniej jeden Obszar.")]
        public ICollection<string> SelectedBranches { get; set; }

        [Display(Name = "Obszar")]
        public IList<SelectListItem> AvailableBranches { get; set; }


        public ICollection<AddMeetingViewModel> MeetingsViewModels { get; set; }
    }
}