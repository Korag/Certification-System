using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayAllCompanyEmployees
    {
        [Display(Name = "Pracownicy przedsiębiorstwa")]
        public ICollection<DisplayCrucialDataUserViewModel> CompanyUserRoleWorker { get; set; }

        [Display(Name = "Managerowie przedsiębiorstwa")]
        public ICollection<DisplayCrucialDataUserViewModel> CompanyUserRoleManager { get; set; }
    }
}