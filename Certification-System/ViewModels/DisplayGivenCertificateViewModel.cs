using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Certification_System.ViewModels
{
    public class DisplayGivenCertificateViewModel
    {
        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany certyfikat")]
        public string Certificate { get; set; }

        [Display(Name = "Użytkownik")]
        public string User { get; set; }

        [Display(Name = "Szkolenie")]
        public string Course { get; set; }
    }
}
