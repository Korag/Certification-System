using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DegreeDetailsViewModel
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


        public ICollection<DisplayListOfCertificatesViewModel> RequiredCertificates { get; set; }
        public ICollection<DisplayDegreeViewModel> RequiredDegrees { get; set; }
        public ICollection<DisplayUsersViewModel> UsersWithDegree { get; set; }
    }
}
