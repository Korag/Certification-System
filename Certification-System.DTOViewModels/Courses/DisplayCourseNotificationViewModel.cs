using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseNotificationViewModel
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string CourseIndexer { get; set; }

        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Kurs po terminie od: [dni]")]
        public int DaysAfterEndDate { get; set; }
    }
}
