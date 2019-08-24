using Certification_System.Entities;
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
    }
}
