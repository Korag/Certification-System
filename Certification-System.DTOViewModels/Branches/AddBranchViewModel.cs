using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddBranchViewModel
    {
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(40, ErrorMessage = "Nazwa obszaru nie powinna być dłuższa niż {1} znaków.")] 
        [Display(Name = "Nazwa obszaru")]
        public string Name { get; set; }
    }
}