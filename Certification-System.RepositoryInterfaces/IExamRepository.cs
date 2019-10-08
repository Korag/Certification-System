using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamRepository
    {
        ICollection<Exam> GetListOfExams();
        ICollection<Exam> GetListOfActiveExams();
        void AddExam(Exam exam);
        ICollection<Exam> GetExamsById(ICollection<string> examsIdentificators);
        Exam GetExamById(string examIdentificator);
        ICollection<Exam> GetExamsByExaminerId(string userIdentificator);
        void UpdateExam(Exam exam);
        ICollection<SelectListItem> GetExamsAsSelectList();
        ICollection<SelectListItem> GetExamsWhichAreDividedToTermsAsSelectList();
        Exam GetExamByExamTermId(string examTermIdentificator);
        ICollection<SelectListItem> GetAddExamMenuOptions();
        Exam GetExamByExamResultId(string examResultId);
        IList<SelectListItem> GetActiveExamsWithVacantSeatsAsSelectList();
        void AddUserToExam(string examIdentificator, string userIdentificator);
        void DeleteUsersFromExam(string examIdentificator, ICollection<string> usersIdentificators);
        void AddUsersToExam(string examIdentificator, ICollection<string> usersIdentificators);
        void SetMaxAmountOfPointsToEarn(string examIdentificator, double maxAmountOfPointsToEarn);
        ICollection<Exam> GetExamPeriods(string examIndexer);
        IList<SelectListItem> GetActiveExamsAsSelectList();
        Exam DeleteExamResultFromExam(string examResultIdentificator);
        Exam DeleteExamTermFromExam(string examTermIdentificator);
        void DeleteExam(string examIdentificator);
        IList<SelectListItem> GetExamsTypesAsSelectList();
        ICollection<Exam> DeleteExams(ICollection<string> examsIdentificators);
        ICollection<Exam> DeleteUserFromExams(string userIdentificator, ICollection<string> examsIdentificators);
        string CountExamsWithIndexerNamePart(string namePartOfIndexer);
        IList<SelectListItem> GetExamsByIdAsSelectList(ICollection<string> examsIdentificators);
        IList<SelectListItem> GetFirstPeriodExamsAsSelectList();
        ICollection<Exam> GetOnlyActiveExamsDividedToTermsById(ICollection<string> examsIdentificators);
        void AddExamsResultsToExam(string examIdentificator, ICollection<string> examsResultsIdentificators);
        Exam GetExamByIndexer(string examIndexer);
    }
}
