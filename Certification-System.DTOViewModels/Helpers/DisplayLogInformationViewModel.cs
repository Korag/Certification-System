using System;

namespace Certification_System.DTOViewModels
{
    public class DisplayLogInformationViewModel
    {
        public DisplayCrucialDataUserViewModel ChangeAuthor { get; set; }

        public string TypeOfAction { get; set; }
        public string DescriptionOfAction { get; set; }
        public DateTime DateOfLogCreation { get; set; }
    }
}
