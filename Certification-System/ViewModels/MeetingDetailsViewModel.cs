using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class MeetingDetailsViewModel
    {
        public string MeetingIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string MeetingIndexer { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Data szkolenia")]
        public DateTime DateOfMeeting { get; set; }

        [Display(Name = "Państwo")]
        public string Country { get; set; }

        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Display(Name = "Lista obecności")]
        public ICollection<string> AttendanceList { get; set; }

        [Display(Name = "Uczestnicy kursu")]
        public ICollection<DisplayUsersViewModel> AllCourseParticipants { get; set; }

        [Display(Name = "Instruktorzy")]
        public ICollection<AddInstructorViewModel> Instructors { get; set; }
    }
}
