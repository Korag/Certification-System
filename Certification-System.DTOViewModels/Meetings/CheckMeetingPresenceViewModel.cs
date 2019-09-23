using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CheckMeetingPresenceViewModel
    {
        public string MeetingIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string MeetingIndexer { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Data rozpoczęcia szkolenia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia szkolenia")]
        public DateTime DateOfEnd { get; set; }

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
        public PresenceCheckBoxViewModel[] AttendanceList { get; set; }

        [Display(Name = "Uczestnicy kursu")]
        public ICollection<DisplayCrucialDataUserViewModel> AllCourseParticipants { get; set; }
    }
}
