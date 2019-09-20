using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GetUserImageViewModel
    {
        public string UserIdentificator { get; set; }

        [Display(Name = "Zdjęcie użytkownika")]
        public IFormFile Image { get; set; }
    }
}
