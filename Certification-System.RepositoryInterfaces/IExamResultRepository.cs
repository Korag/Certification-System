using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamResultRepository
    {
        ICollection<ExamResult> GetListOfExamsResults();
        void AddExamResult(ExamResult examResult);
        void AddExamsResults(ICollection<ExamResult> examsResults);
        ICollection<ExamResult> GetExamsResultsById(ICollection<string> examResultsIdentificators);
        ICollection<ExamResult> GetExamsResultsByExamTermId(string examTermIdentificator);
        ExamResult GetExamResultById(string examResultIdentificator);
        void DeleteExamResult(string examResultIdentificator);
        ICollection<ExamResult> DeleteExamsResultsByExamTermId(string examTermIdentificator);
        ICollection<ExamResult> DeleteExamsResults(ICollection<string> examsResultsIdentificators);
        ICollection<ExamResult> DeleteExamsResultsByUserId(string userIdentificator);
        string CountExamsResultsWithIndexerNamePart(string namePartOfIndexer);
    }
}
