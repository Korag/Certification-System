using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class EditGivenCertificateViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Identyfikator nadawanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }
    }
}
