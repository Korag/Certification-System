namespace Certification_System.DTOViewModels
{
    public class DisplayGivenCertificateWithUserDataTableViewModel
    {
        public DisplayCrucialDataWithBirthDateUserViewModel User { get; set; }
        public DisplayGivenCertificateToUserWithoutCourseViewModel GivenCertificate { get; set; }
        public DataTableOptionsViewModel Options { get; set; }
    }
}
