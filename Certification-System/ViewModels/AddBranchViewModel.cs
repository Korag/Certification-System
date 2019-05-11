using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class AddBranchViewModel
    {
        [Required]
        [Display(Name = "Nazwa obszaru")]
        public string Name { get; set; }
    }
}