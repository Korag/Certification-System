using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class EditGivenDegreeViewModel
    {
        public string GivenDegreeIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}")]
        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Użytkownik")]
        public DisplayCrucialDataUserViewModel User { get; set; }

        [Display(Name = "Stopień zawodowy")]
        public DisplayCrucialDataDegreeViewModel Degree { get; set; }
    }
}
