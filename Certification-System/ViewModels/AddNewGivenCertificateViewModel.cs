using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class AddNewGivenCertificateViewModel
    {
        [Required]
        [Display(Name = "Identyfikator nadawanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Required]
        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }


        [Display(Name = "Certyfikaty")]
        public IList<SelectListItem> AvailableCertificates { get; set; }

        [Required]
        public string SelectedCertificate { get; set; }

        [Display(Name = "Użytkownicy")]
        public IList<SelectListItem> AvailableUsers{ get; set; }

        [Required]
        public string SelectedUser { get; set; }

        [Display(Name = "Szkolenia")]
        public IList<SelectListItem> AvailableCourses { get; set; }

        [Required]
        public string SelectedCourses { get; set; }
    }
}
