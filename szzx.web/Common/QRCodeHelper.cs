using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class QRCodeHelper
    {
        public static void GenerateQRCode(string text, string qrImgPath)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            var dir = Path.GetDirectoryName(qrImgPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            qrCodeImage.Save(qrImgPath);
        }
    }
}