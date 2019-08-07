using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenDegreeToUserViewModel
    {
        public string GivenDegreeIdentificator { get; set; }

        [Display(Name = "Identyfikator nadanego stopnia zawodowego")]
        public string GivenDegreeIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany stopień zawodowy")]
        public DisplayCrucialDataDegreeViewModel Degree { get; set; }
    }
}
