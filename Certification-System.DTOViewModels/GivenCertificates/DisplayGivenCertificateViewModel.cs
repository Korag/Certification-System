using Certification_System.DTOViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany certyfikat")]
        public DisplayCrucialDataCertificateViewModel Certificate { get; set; }

        [Display(Name = "Użytkownik")]
        public DisplayCrucialDataUsersViewModel User { get; set; }

        [Display(Name = "Szkolenie")]
        public DisplayCrucialDataCourseViewModel Course { get; set; }
    }
}
