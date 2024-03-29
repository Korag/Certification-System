﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddGivenDegreeViewModel
    {
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

        [Display(Name = "Użytkownicy")]
        public IList<SelectListItem> AvailableUsers{ get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedUser { get; set; }

        [Display(Name = "Stopnie zawodowe")]
        public IList<SelectListItem> AvailableDegrees{ get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        public string SelectedDegree { get; set; }
    }
}
