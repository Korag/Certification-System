using Certification_System.ServicesInterfaces;
using QRCoder;
using System.Drawing;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace Certification_System.Services
{
    public class GeneratorQR : IGeneratorQR
    {
        public Bitmap GenerateQRCode(string Url, string pathToIcon)
        {
            Url generator = new Url(Url);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap icon = new Bitmap(pathToIcon, true);

            Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.White, icon);

            return qrCodeAsBitmap;
        }

        public byte[] GenerateQRCodeFromGivenURL(string URL, string pathToIcon)
        {
            var QRBitmap = GenerateQRCode(URL, pathToIcon);

            using (MemoryStream stream = new MemoryStream())
            {
                QRBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var ByteArray = stream.ToArray();

                return ByteArray;
            }
        }
    }
}
