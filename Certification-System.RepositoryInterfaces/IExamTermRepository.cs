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
        ICollection<ExamTerm> DeleteExamsTerms(ICollection<string> examsTermsIdentificators);
        IList<SelectListItem> GetActiveExamTermsWithVacantSeatsAsSelectList(Exam exam);
        IList<SelectListItem> GetActiveExamTermsWithVacantSeatsAsSelectList();
        void AddUserToExamTerm(string examTermIdentificator, string userIdentificator);
        ExamTerm DeleteUserFromExamTerm(string examTermIdentificator, string userIdentificator);
        void DeleteUsersFromExamTerms(ICollection<string> examTermsIdentificators, ICollection<string> usersIdentificators);
        void DeleteUsersFromExamTerm(string examTermIdentificator, ICollection<string> usersIdentificators);
        void AddUsersToExamTerm(string examTermIdentificator, ICollection<string> usersIdentificators);
        void DeleteExamTerm(string examTermIdentificator);
        ICollection<ExamTerm> DeleteUserFromExamsTerms(string userIdentificator, ICollection<string> examsTermsIdentificators);
        string CountExamsTermsWithIndexerNamePart(string namePartOfIndexer);
    }
}
