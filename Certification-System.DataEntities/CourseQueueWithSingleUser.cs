namespace Certification_System.Entities
{
    public class CourseQueueWithSingleUser
    {
        public string CourseIdentificator { get; set; }
        public string UserIdentificator { get; set; }
        public LogInformation LogDataOfAssignToCourseQueue { get; set; }
    }
}