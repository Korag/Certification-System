using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class UserGivenDegreePossessionConfirmationViewModel
    {
        public string UserIdentificator { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] QRCode;

        public string GivenDegreeIdentificator { get; set; }
        public string ReceiptDate { get; set; }
        public string ExpirationDate { get; set; }

        public string DegreeIdentificator { get; set; }
        public string DegreeName { get; set; }
        public string DegreeDescription { get; set; }
        public ICollection<string> Branches { get; set; }
    }
}
