using System;

namespace Certification_System.Entities
{
    public class CourseQueueUser
    {
        public CourseQueueUser()
        {
            DateOfApplication = DateTime.Now;
        }

        public string UserIdentificator { get; set; }
        public DateTime DateOfApplication { get; set; }
    }
}