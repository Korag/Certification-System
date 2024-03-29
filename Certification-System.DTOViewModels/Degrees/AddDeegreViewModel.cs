﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddDegreeViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Nazwa stopnia zawodowego")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        [MaxLength(255)]
        public string Description { get; set; }

        [Display(Name = "Warunki uzyskania")]
        public string ConditionsList { get; set; }

        [Required(ErrorMessage = "Należy zaznaczyć conajmniej jeden Obszar.")]
        public ICollection<string> SelectedBranches { get; set; }

        [Display(Name = "Obszary stopnia zawodowego")]
        public IList<SelectListItem> AvailableBranches { get; set; }

        public ICollection<string> SelectedCertificates { get; set; }

        [Display(Name = "Wymagane certyfikaty")]
        public IList<SelectListItem> AvailableCertificates { get; set; }

        public ICollection<string> SelectedDegrees{ get; set; }

        [Display(Name = "Wymagane stopnie zawodowe")]
        public IList<SelectListItem> AvailableDegrees { get; set; }
    }
}
