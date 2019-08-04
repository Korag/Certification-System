using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayMeetingViewModel
    {
        public string MeetingIdentificator { get; set; }

        public string CourseIdentificator { get; set; }

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

        [Display(Name = "Instruktorzy")]
        public ICollection<string> InstructorsCredentials { get; set; }

        public ICollection<string> InstructorsIdentificators { get; set; }

        [Display(Name = "Identyfikator kursu")]
        public string CourseIndexer { get; set; }

        [Display(Name = "Nazwa kursu")]
        public string CourseName { get; set; }
    }
}
