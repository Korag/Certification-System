using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamTermRepository
    {
        ICollection<ExamTerm> GetListOfExamsTerms();
        void AddExamTerm(ExamTerm examTerm);
        void AddExamsTerms(ICollection<ExamTerm> examsTerms);
        ExamTerm GetExamTermById(string examTermIdentificator);
        ICollection<ExamTerm> GetExamTermsById(ICollection<string> examsTermsIdentificators);
        ICollection<ExamTerm> GetExamTermsByExaminerId(string userIdentificator);
    }
}
