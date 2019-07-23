using System.Drawing;
using System;

namespace Certification_System.ServicesInterfaces.IGeneratorQR
{
    public interface IGeneratorQR
    {
       Bitmap GenerateQRCode(string Url);
    }
}
