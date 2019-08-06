using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CertificateDetailsViewModel
    {
        public string CertificateIdentificator { get; set; }

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

        [Display(Name = "Użytkownicy posiadający certyfikat")]
        public ICollection<DisplayCrucialDataWithCompaniesRoleViewModel> UsersWithCertificate { get; set; }

        [Display(Name = "Kursy, które zakończyły się nadaniem takiego certyfikatu")]
        public ICollection<DisplayCourseViewModel> CoursesWhichEndedWithCertificate { get; set; }
    }
}
