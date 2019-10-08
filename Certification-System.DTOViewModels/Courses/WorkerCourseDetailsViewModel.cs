using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class WorkerCourseDetailsViewModel
    {
        [Display(Name = "Kurs")]
        public DisplayCourseViewModel Course { get; set; }

        [Display(Name = "Spotkania w ramach szkolenia")]
        public ICollection<DisplayMeetingWithoutCourseViewModel> Meetings { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayExamWithoutCourseViewModel> Exams { get; set; }

        [Display(Name = "Zestawienie obecności na spotkaniach")]
        public ICollection<DisplayMeetingWithUserPresenceInformation> MeetingsPresence { get; set; }

        [Display(Name = "Egzaminy użytkownika")]
        public ICollection<DisplayExamResultToUserViewModel> UserExamWithExamResults { get; set; }

        [Display(Name = "Egzaminy, które użytkownik musi zdać")]
        public ICollection<DisplayCrucialDataExamViewModel> UserNotPassedExams { get; set; }

        [Display(Name = "Nadane certyfikaty za wybrany kurs")]
        public ICollection<DisplayGivenCertificateToUserViewModel> GivenCertificates { get; set; }

        [Display(Name = "Trwające egzaminy na które zapisany jest użytkownik")]
        public ICollection<string> UserLastingExamsIndexers { get; set; }
    }
}
