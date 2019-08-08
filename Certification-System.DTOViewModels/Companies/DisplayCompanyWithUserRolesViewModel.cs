using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyWithUserRolesViewModel
    {
        public string CompanyIdentificator { get; set; }

        [Display(Name = "Nazwa przedsiębiorstwa")]
        public string CompanyName { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

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

        [Display(Name = "Rola użytkownika w przesiębiorstwie")]
        public ICollection<string> CompanyRoles { get; set; }
    }
}
