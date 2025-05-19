using QRCoder;
using System.Drawing;
using System.IO;

namespace LibraryManagement.Services.Payments.QRCodeServices
{
    public class QRCodeService
    {
        public string GenerateSvgQRCode(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            SvgQRCode qrCode = new SvgQRCode(qrCodeData);
            string svgImage = qrCode.GetGraphic(4);
            return svgImage;
        }
    }
}