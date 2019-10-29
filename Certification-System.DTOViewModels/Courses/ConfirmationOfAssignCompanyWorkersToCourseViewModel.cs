using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class ConfirmationOfAssignCompanyWorkersToCourse
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Pracownicy mający zostać zapisani na kurs")]
        public ICollection<DisplayCrucialDataUserViewModel> CompanyWorkers { get; set; }

        [Display(Name = "Dane kursu")]
        public DisplayCourseOfferViewModel Course { get; set; }

        [Display(Name = "Cena")]
        public double Price { get; set; }

        [Display(Name = "Sumaryczna cena")]
        public double OverallPrice { get; set; }

        public string Code { get; set; }
    }
}
