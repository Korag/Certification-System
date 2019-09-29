using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserWithCourseResultsViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Wyniki egzaminów")]
        public ICollection<DisplayExamResultWithExamIdentificator> ExamsResults { get; set; }

        [Display(Name = "Ilość obecności")]
        public int QuantityOfPresenceOnMeetings { get; set; }

        [Display(Name = "Ilość spotkań")]
        public int QuantityOfMeetings { get; set; }

        [Display(Name = "Procentowa obecność na spotkaniach")]
        public double PercentageOfUserPresenceOnMeetings { get; set; }
    }
}