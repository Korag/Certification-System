using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenDegreeDetailsForAnonymousViewModel
    {
        public string GivenDegreeIdentificator { get; set; }

        [Display(Name = "Identyfikator nadanego stopnia zawodowego")]
        public string GivenDegreeIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Szablon stopnia zawodowego")]
        public DisplayDegreeWithoutRequirementsViewModel Degree { get; set; }

        [Display(Name = "Posiadacz stopnia zawodowego")]
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
