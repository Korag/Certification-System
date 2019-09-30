using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayUserListWithExamResultsAndExamIdentificator
    {
        public string ExamIdentificator { get; set; }
        public string ExamTermIdentificator { get; set; }

        public ICollection<DisplayUserWithExamResults> ResultsList { get; set; }
    }
}