using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class AddExamComponentViewModel
    {
        public int Iterator { get; set; }

        public IList<SelectListItem> AvailableExaminers { get; set; }

        [Display(Name = "Terminy")]
        public IList<AddExamTermWithoutExamViewModel> ExamTerms { get; set; }
    }
}
