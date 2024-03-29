﻿using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateWithUserDataTableViewModel
    {
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }
        public ICollection<DisplayGivenCertificateToUserWithoutCourseViewModel> GivenCertificates { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
