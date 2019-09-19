using Microsoft.AspNetCore.Http;
using System.IO;

namespace Certification_System.DTOViewModels
{
    public class UserIdentificatorWithQRViewModel
    {
        public string UserIdentificator { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] QRCode;
        public byte[] UserImage;
    }
}
