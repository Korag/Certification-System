using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenDegreeViewModel
    {
        public string GivenDegreeIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string GivenDegreeIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany stopień zawodowy")]
        public DisplayCrucialDataDegreeViewModel Degree { get; set; }

        [Display(Name = "Użytkownik")]
        public DisplayCrucialDataUsersViewModel User { get; set; }
    }
}
