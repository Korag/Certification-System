using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class EditGivenCertificateViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Required]
        [Display(Name = "Identyfikator nadawanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Required]
        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }
    }
}
