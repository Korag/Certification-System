using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class AddCompanyViewModel
    {
        [Required]
        [Display(Name = "Nazwa przedsiębiorstwa")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Adres email")]
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