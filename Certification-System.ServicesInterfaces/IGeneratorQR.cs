using System.Drawing;
using System;

namespace Certification_System.ServicesInterfaces
{
    public interface IGeneratorQR
    {
       Bitmap GenerateQRCode(string Url);
    }
}
