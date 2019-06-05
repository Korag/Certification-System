using System.ComponentModel.DataAnnotations;

namespace Certification_System.ViewModels
{
    public class VerifyUserViewModel
    {
        [Display(Name="Identyfikator użytkownika")]
        public string UserIdentificator { get; set; }
    }
}
