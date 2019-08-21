using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCrucialDataCourseViewModel
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Identyfikator kursu")]
        public string CourseIndexer { get; set; }

        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis kursu")]
        public string Description { get; set; }
    }
}
