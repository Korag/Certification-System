using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayRejectedUserLog
    {
        public DisplayCrucialDataCourseViewModel Course { get; set; }
        public DisplayCrucialDataUserViewModel User { get; set; }

        [Display(Name = "Data zapisania")]
        public DateTime DateOfAssignToCourseQueue { get; set; }

        [Display(Name = "Data odrzucenia")]
        public DateTime DateOfRejection { get; set; }
    }
}
