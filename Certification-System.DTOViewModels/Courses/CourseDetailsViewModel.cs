using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CourseDetailsViewModel
    {
        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Spotkania w ramach szkolenia")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }

        [Display(Name = "Zarejestrowani uczestnicy")]
        public ICollection<DisplayCrucialDataUserViewModel> EnrolledUsers { get; set; }

        [Display(Name = "Instruktorzy")]
        public ICollection<DisplayCrucialDataWithContactUserViewModel> Instructors { get; set; }

        [Display(Name = "Egzaminatorzy")]
        public ICollection<DisplayCrucialDataWithContactUserViewModel> Examiners { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayExamWithoutCourseViewModel> Exams { get; set; }

        [Display(Name = "Lista przyznanych certyfikatów")]
        public DispenseGivenCertificateCheckBoxViewModel[] DispensedGivenCertificates { get; set; }
    }
}
