using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class InstructorExaminerDetailsViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Role użytkownika")]
        public ICollection<string> Roles { get; set; }

        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Kursy")]
        public ICollection<DisplayCourseWithUserRoleViewModel> Courses{ get; set; }

        [Display(Name = "Spotkania")]
        public ICollection<DisplayMeetingWithoutInstructorViewModel> Meetings { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayExamWithoutExaminerViewModel> Exams { get; set; }

        [Display(Name = "Tury egzaminów")]
        public ICollection<DisplayExamTermWithoutExaminerViewModel> ExamsTerms { get; set; }
    }
}
