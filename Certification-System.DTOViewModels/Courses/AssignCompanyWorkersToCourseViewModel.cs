using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AssignCompanyWorkersToCourseViewModel
    {
        [Display(Name = "Kurs")]
        public DisplayCourseOfferViewModel Course { get; set; }

        [Display(Name = "Koszt kursu")]
        public double Price { get; set; }

        [Display(Name = "Liczba wolnych miejsc")]
        public int VacantSeats { get; set; }

        [Display(Name = "Lista użytkowników możliwych do dodania do kursu")]
        public AddUsersFromCheckBoxViewModel[] CompanyWorkersToAssignToCourse { get; set; }

        [Display(Name = "Niezapisani na kurs pracownicy przedsiębiorstwa")]
        public ICollection<DisplayCrucialDataUserViewModel> CompanyWorkers { get; set; }
    }
}
