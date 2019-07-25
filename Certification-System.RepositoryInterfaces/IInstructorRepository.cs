using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IInstructorRepository
    {
        ICollection<Instructor> GetInstructorsById(ICollection<string> InstructorsId);
        Instructor GetInstructorById(string instructorIdentificator);
        void AddInstructor(Instructor instructor);
        ICollection<Instructor> GetInstructors();
        ICollection<SelectListItem> GetInstructorsAsSelectList();
    }
}
