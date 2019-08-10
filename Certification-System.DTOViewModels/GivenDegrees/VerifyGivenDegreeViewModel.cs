using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class VerifyGivenDegreeViewModel
    {
        [Display(Name = "Identyfikator nadanego stopnia zawodowego")]
        public string GivenDegreeIdentificator { get; set; }

        public bool GivenDegreeIdentificatorNotExist { get; set; }
        public bool GivenDegreeIdentificatorBadFormat { get; set; }
    }
}
