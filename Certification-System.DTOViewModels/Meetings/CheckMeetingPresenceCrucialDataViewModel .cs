using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class CheckMeetingPresenceCrucialDataViewModel
    {
        public string MeetingIdentificator { get; set; }

        [Display(Name = "Lista obecności")]
        public PresenceCheckBoxViewModel[] AttendanceList { get; set; }
    }
}
