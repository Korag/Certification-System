using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class WorkerCompetencesViewModel
    {
        public string UserIdentificator { get; set; }

        public ICollection<DisplayGivenCertificateToUserViewModel> GivenCertificates { get; set; }
        public ICollection<DisplayGivenDegreeToUserViewModel> GivenDegrees { get; set; }
    }
}