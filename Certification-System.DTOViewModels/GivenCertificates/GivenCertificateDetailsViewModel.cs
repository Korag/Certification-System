using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GivenCertificateDetailsViewModel
    {
        public string GivenCertificateIdentificator { get; set; }

        [Display(Name = "Identyfikator nadanego certyfikatu")]
        public string GivenCertificateIndexer { get; set; }

        [Display(Name = "Data otrzymania")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Data wygaśnięcia")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Nadany certyfikat")]
        public DisplayCertificateViewModel Certificate { get; set; }

        [Display(Name = "Dane właściciela certyfikatu")]
        public DisplayAllUserInformationViewModel User { get; set; }

        [Display(Name = "Przedsiębiorstwa")]
        public ICollection<DisplayCompanyViewModel> Companies { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Instruktorzy")]
        public ICollection<DisplayAllUserInformationViewModel> Instructors { get; set; }


        [Display(Name = "Spotkania w ramach kursu")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }
    }
}
