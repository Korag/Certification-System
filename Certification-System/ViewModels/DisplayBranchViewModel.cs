using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class DisplayBranchViewModel
    {
        [Display(Name = "Identyfikator")]
        public string BranchIdentificator { get; set; }

        [Display(Name = "Nazwa obszaru")]
        public string Name { get; set; }
    }
}
