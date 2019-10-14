using System;

namespace Certification_System.Entities
{
    public class CourseQueueUser
    {
        public string CourseQueueUserIdentificator { get; set; }

        public string UserIdentificator { get; set; }
        public DateTime DateOfApplication { get; set; }
    }
}