using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Certification_System.ViewModels
{
    public class AddBranchViewModel
    {
        [Required]
        [Display(Name = "Nazwa obszaru")]
        public string Name { get; set; }
    }
}