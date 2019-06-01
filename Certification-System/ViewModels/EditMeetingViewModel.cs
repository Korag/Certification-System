using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class EditMeetingViewModel
    {
        public string MeetingIdentificator { get; set; }

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

        [Display(Name = "Instruktorzy")]
        public IList<SelectListItem> AvailableInstructors { get; set; }


        [Display(Name = "Instruktor")]
        [Required(ErrorMessage = "Należy wybrać conajmniej jednego instruktora.")]
        public ICollection<string> SelectedInstructors { get; set; }
    }
}
