using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Certification_System.ViewModels
{
    public class AddCertificateToDbViewModel
    {
        [Required]
        [Display(Name="Identyfikator")]
        public string CertificateIndexer { get; set; }
        [Required]
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