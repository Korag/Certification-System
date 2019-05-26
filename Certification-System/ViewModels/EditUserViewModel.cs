﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class EditUserViewModel
    {
        public string UserIdentificator { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Imię powinno zawierać od 3 do 100 znaków.")]
        [DataType(DataType.Text)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwisko powinno składać się od 3 do 100 znaków")]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa kraju powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa miasta powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Kod pocztowy")]
        public string PostCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa ulicy powinna mieć długość pomiędzy 3 a 100 znakami")]
        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Numer domu/mieszkania nie powinien być większy niż 10 znaków")]
        [Display(Name = "Numer domu/mieszkania")]
        public string NumberOfApartment { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Data urodzenia")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Rola użytkownika")]
        public IList<SelectListItem> AvailableRoles { get; set; }

        [Required(ErrorMessage = "Należy przydzielić rolę")]
        public ICollection<string> SelectedRole { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public IList<SelectListItem> AvailableCompanies { get; set; }

        [Display(Name = "Przedsiębiorstwo zarządzane przez użytkownika")]
        public ICollection<string> CompanyRoleManager { get; set; }

        [Display(Name = "Przedsiębiorstwo zrzeszające pracownika")]
        public ICollection<string> CompanyRoleWorker { get; set; }
    }
}