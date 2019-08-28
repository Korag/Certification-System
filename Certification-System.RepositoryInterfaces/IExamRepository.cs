using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamRepository
    {
        ICollection<Exam> GetListOfExams();
        void AddExam(Exam exam);
        ICollection<Exam> GetExamsById(ICollection<string> examsIdentificators);
        Exam GetExamById(string examIdentificator);
        ICollection<Exam> GetExamsByExaminatorId(string userIdentificator);
        void UpdateExam(Exam exam);
        ICollection<SelectListItem> GetExamsAsSelectList();
        ICollection<SelectListItem> GetExamsWhichAreDividedToTermsAsSelectList();
        Exam GetExamByExamTermId(string examTermIdentificator);
        ICollection<SelectListItem> GetAddExamMenuOptions();
    }
}
