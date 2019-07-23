using Certification_System.ServicesInterfaces.IGeneratorQR;
using QRCoder;
using System.Drawing;
using static QRCoder.PayloadGenerator;

namespace Certification_System.Services
{
    public class GeneratorQR : IGeneratorQR
    {
        public Bitmap GenerateQRCode(string Url)
        {
            Url generator = new Url(Url);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20);

            return qrCodeAsBitmap;
        }
    }
}
