using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DeleteUsersFromCourseViewModel
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Lista użytkowników do usunięcia z kursu")]
        public DeleteUsersFromCourseCheckBoxViewModel[] UsersToDeleteFromCourse { get; set; }

        [Display(Name = "Uczestnicy kursu")]
        public ICollection<DisplayCrucialDataUserViewModel> AllCourseParticipants { get; set; }
    }
}
