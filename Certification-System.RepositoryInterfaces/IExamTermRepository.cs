using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamTermRepository
    {
        ICollection<ExamTerm> GetListOfExamsTerms();
        void AddExamTerm(ExamTerm examTerm);
        void AddExamsTerms(ICollection<ExamTerm> examsTerms);
        ExamTerm GetExamTermById(string examTermIdentificator);
        ICollection<ExamTerm> GetExamsTermsById(ICollection<string> examsTermsIdentificators);
        ICollection<ExamTerm> GetExamTermsByExaminerId(string userIdentificator);
        void UpdateExamsTerms(ICollection<ExamTerm> examTerms);
        void UpdateExamTerm(ExamTerm examTerm);
        void DeleteExamsTerms(ICollection<string> examsTermsIdentificators);
        IList<SelectListItem> GetActiveExamTermsWithVacantSeatsAsSelectList(Exam exam);
        void AddUserToExamTerm(string examTermIdentificator, string userIdentificator);
        void DeleteUsersFromExamTerms(ICollection<string> examTermsIdentificators, ICollection<string> usersIdentificators);
    }
}
