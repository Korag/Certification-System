using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingWithUserPresenceInformation
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

        [Display(Name = "Obecność na spotkaniu")]
        public bool IsUserPresent { get; set; }
    }
}
