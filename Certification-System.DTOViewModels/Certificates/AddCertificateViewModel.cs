using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddCertificateViewModel
    {
        public string CertificateIdentificator { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name="Identyfikator")]
        public string CertificateIndexer { get; set; }

        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [StringLength(100, ErrorMessage = "Pole \"{0}\" musi mieć długość conajmniej {2} znaków.", MinimumLength = 6)]
        [Display(Name = "Nazwa certyfikatu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        [MaxLength(255)]
        public string Description { get; set; }

        [Display(Name = "Przedawniony")]
        public bool Depreciated { get; set; }

        [Required(ErrorMessage = "Należy zaznaczyć conajmniej jeden Obszar.")]
        public ICollection<string> SelectedBranches{ get; set; }

        [Display(Name = "Obszar")]
        public IList<SelectListItem> AvailableBranches { get; set; }
    }
}