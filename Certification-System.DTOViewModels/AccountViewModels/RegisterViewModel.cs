﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [EmailAddress(ErrorMessage = "W polu \"{0}\" nie znajduje się poprawny adres email.")]
        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć długość conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Wprowadzone hasła różnią się.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Imię powinno zawierać od 3 do 100 znaków.")]
        [DataType(DataType.Text)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwisko powinno składać się od 3 do 100 znaków")]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa kraju powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa miasta powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa ulicy powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Numer domu/mieszkania nie powinien być większy niż 10 znaków")]
        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }
    }
}
