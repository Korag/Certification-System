using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Certification_System.ViewModels
{
    public class AddInstructorViewModel
    {
        [Required]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string Phone { get; set; }
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
    }
}