using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ICourseRepository
    {
        void AddCourse(Course course);
        void UpdateCourse(Course course);
        Course GetCourseById(string courseIdentificator);
        ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators);
        ICollection<SelectListItem> GetCoursesAsSelectList();
        ICollection<Course> GetActiveCourses();
        Course GetCourseByMeetingId(string meetingIdentificator);
        ICollection<Course> GetCourses();
        void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator);
    }
}
