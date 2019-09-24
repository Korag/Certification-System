namespace Certification_System.DTOViewModels
{
    public class DeleteUsersFromCourseCrucialViewModel
    {
        public string CourseIdentificator { get; set; }

        public DeleteUsersFromCheckBoxViewModel[] UsersToDeleteFromCourse { get; set; }
    }
}
