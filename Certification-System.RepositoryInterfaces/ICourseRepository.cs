using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface ICourseRepository
    {
        ICollection<Course> GetListOfCourses();
        void AddCourse(Course course);
        void UpdateCourse(Course course);
        Course GetCourseById(string courseIdentificator);
        ICollection<Course> GetCoursesById(ICollection<string> coursesIdentificators);
        ICollection<SelectListItem> GetActiveCoursesAsSelectList();
        ICollection<SelectListItem> GetAllCoursesAsSelectList();
        ICollection<Course> GetActiveCourses();
        Course GetCourseByMeetingId(string meetingIdentificator);
        void AddMeetingToCourse(string meetingIdentificator, string courseIdentificator);
        void AddEnrolledUsersToCourse(string courseIdentificator, ICollection<string> usersIdentificators);
        ICollection<SelectListItem> GetActiveCoursesWithVacantSeatsAsSelectList();
        ICollection<Course> GetCoursesByMeetingsId(ICollection<string> meetingsIdentificators);
        void DeleteUsersFromCourse(string courseIdentificator, ICollection<string> usersIdentificators);
        ICollection<Course> GetExaminerCourses(string userIdentificator, ICollection<Exam> exams);
    }
}
