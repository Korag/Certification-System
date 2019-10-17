using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserLogViewModel
    {
        public DisplayCrucialDataUserViewModel CurrentUser { get; set; }
        public ICollection<DisplayLogInformationViewModel> LogData { get; set; }
    }
}