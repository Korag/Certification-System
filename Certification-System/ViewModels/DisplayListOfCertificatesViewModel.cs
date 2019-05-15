using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Certification_System.ViewModels
{
    public class DisplayListOfCertificatesViewModel
    {
        [Display(Name = "Identyfikator")]
        public string CertificateIndexer { get; set; }

        [Display(Name = "Nazwa certyfikatu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Przedawniony")]
        public bool Depreciated { get; set; }

        [Display(Name = "Obszary")]
        public ICollection<string> Branches { get; set; }
    }
}
