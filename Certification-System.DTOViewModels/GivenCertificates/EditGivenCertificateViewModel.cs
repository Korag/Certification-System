using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class EditGivenCertificateViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Użytkownik")]
        public DisplayCrucialDataUserViewModel User { get; set; }

        [Display(Name = "Certyfikat")]
        public DisplayCrucialDataCertificateViewModel Certificate { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCrucialDataCourseViewModel Course { get; set; }
    }
}
