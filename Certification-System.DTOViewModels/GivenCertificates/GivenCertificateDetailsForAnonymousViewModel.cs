using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenCertificateDetailsForAnonymousViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany certyfikat")]
        public DisplayCrucialDataCertificateViewModel Certificate { get; set; }

        [Display(Name = "Dane właściciela certyfikatu")]
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }
    }
}
