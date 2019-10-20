using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayLogInformationViewModel
    {
        public DisplayCrucialDataUserViewModel ChangeAuthor { get; set; }

        [Display(Name = "Opis operacji")]
        public string DescriptionOfAction { get; set; }

        [Display(Name = "Dodatkowe informacje")]
        public string AdditionalInfo { get; set; }

        [Display(Name = "Data operacji")]
        public DateTime DateOfLogCreation { get; set; }
    }
}
