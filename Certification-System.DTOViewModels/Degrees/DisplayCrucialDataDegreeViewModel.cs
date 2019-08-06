using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataDegreeViewModel
    {
        public string DegreeIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string DegreeIndexer { get; set; }

        [Display(Name = "Nazwa stopnia zawodowego")]
        public string Name { get; set; }
    }
}
