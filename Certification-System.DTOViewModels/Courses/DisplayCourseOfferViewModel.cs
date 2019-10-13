using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCourseOfferViewModel
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

        [Display(Name = "Wolne miejsca")]
        public int VacantSeats { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<string> Branches { get; set; }

    }
}
