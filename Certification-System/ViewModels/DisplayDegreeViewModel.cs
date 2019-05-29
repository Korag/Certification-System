using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class DisplayDegreeViewModel
    {
        public string DegreeIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string DegreeIndexer { get; set; }

        [Display(Name = "Nazwa stopnia zawodowego")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Obszary stopnia zawodowego")]
        public ICollection<string> Branches { get; set; }

        [Display(Name = "Wymagane certyfikaty")]
        public ICollection<string> RequiredCertificates { get; set; }

        [Display(Name = "Wymagane stopnie zawodowe")]
        public ICollection<string> RequiredDegrees { get; set; }
    }
}
