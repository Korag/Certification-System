using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithCourseExamPeriodsResultsViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Niezdane egzaminy")]
        public ICollection<string> LastingExamsIndexers { get; set; }

        [Display(Name = "Zaliczone egzaminy")]
        public ICollection<string> ExamsIndexersPassed { get; set; }
    }
}