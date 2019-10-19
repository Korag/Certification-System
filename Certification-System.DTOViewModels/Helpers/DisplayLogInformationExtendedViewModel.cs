using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayLogInformationExtendedViewModel
    {
        public DisplayCrucialDataUserViewModel ChangeAuthor { get; set; }

        public DisplayCrucialDataUserViewModel SubjectUser { get; set; }

        [Display(Name = "Opis operacji")]
        public string DescriptionOfAction { get; set; }

        [Display(Name = "Szczegóły")]
        public string UrlToDetailsOfAction { get; set; }

        [Display(Name = "Data operacji")]
        public DateTime DateOfLogCreation { get; set; }
    }
}
