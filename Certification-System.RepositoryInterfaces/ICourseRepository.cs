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
        Course GetCourseByExamId(string examIdentificator);
        ICollection<SelectListItem> GetActiveCoursesWhereExamIsRequiredAsSelectList();
        ICollection<Course> GetCoursesByExamsId(ICollection<string> examsIdentificators);
        ICollection<SelectListItem> GenerateSelectList(ICollection<string> coursesIdentificators);
        ICollection<Course> DeleteBranchFromCourses(string branchIdentificator);
        Course DeleteExamFromCourse(string examIdentificator);
        Course DeleteMeetingFromCourse(string meetingIdentificator);
        void DeleteCourse(string courseIdentificator);
        ICollection<Course> DeleteUserFromCourses(ICollection<string> usersIdentificators);
        string CountCoursesWithIndexerNamePart(string namePartOfIndexer);
        ICollection<Course> GetListOfNotStartedCourses();


        ICollection<CourseQueue> GetListOfCoursesQueue();
        CourseQueue GetCourseQueueById(string courseIdentificator);
        ICollection<CourseQueue> GetCoursesQueueById(ICollection<string> coursesIdentificators);
        CourseQueue CreateCourseQueue(string courseIdentificator);
        CourseQueue AddAwaitingUserToCourseQueue(string courseIdentificator, string userIdentificator);
    }
}
