using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ICourseQueueRepository
    {
        ICollection<CourseQueue> GetListOfCoursesQueue();
    }
}
