using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DispenseGivenCertificatesDataTableViewModel
    {
        public ICollection<DisplayUserWithCourseResultsViewModel> Users { get; set; }
        public DispenseGivenCertificateCheckBoxViewModel[] DispensedGivenCertificates { get; set; }
        public ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> LastExamsPeriods { get; set; }
        public bool CourseEnded { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
