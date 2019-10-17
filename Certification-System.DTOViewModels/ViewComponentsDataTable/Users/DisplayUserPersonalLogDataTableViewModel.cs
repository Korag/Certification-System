using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserPersonalLogDataTableViewModel
    {
        public ICollection<DisplayLogInformationViewModel> Logs { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}