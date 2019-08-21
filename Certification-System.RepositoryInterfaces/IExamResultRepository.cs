using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamResultRepository
    {
        ICollection<ExamResult> GetListOfExamsResults();
        void AddExamResult(ExamResult examResult);
        void AddExamsResults(ICollection<ExamResult> examsResults);
    }
}
