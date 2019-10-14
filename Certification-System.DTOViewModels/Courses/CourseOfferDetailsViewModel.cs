using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CourseOfferDetailsViewModel
    {
        public bool UserInQueue { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCourseOfferViewModel Course { get; set; }

        [Display(Name = "Koszt kursu")]
        public double Price { get; set; }

        [Display(Name = "Instruktorzy")]
        public ICollection<DisplayCrucialDataUserViewModel> Instructors { get; set; }

        [Display(Name = "Egzaminatorzy")]
        public ICollection<DisplayCrucialDataUserViewModel> Examiners { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayCrucialDataExamViewModel> Exams { get; set; }
    }
}
