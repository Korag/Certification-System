﻿using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class AddBranchViewModel
    {
        [Display(Name = "Identyfikator")]
        public string BranchIdentificator { get; set; }

        [Required]
        [Display(Name = "Nazwa obszaru")]
        public string Name { get; set; }
    }
}