using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CourseQueueNotificationViewModel
    {
        public DisplayCrucialDataCourseViewModel Course { get; set; }
        public DisplayCrucialDataUserViewModel EnrolledUser { get; set; }

        [Display(Name = "Data zapisania")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Akcja starsza niż 2 tygodnie")]
        public bool EnrollmentOlderThan2Weeks { get; set; }
    }
}
