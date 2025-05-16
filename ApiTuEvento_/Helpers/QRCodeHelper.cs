using QRCoder;

namespace ApiTuEvento_.Helpers
{
    public static class QRCodeHelper
    {
        public static string GenerarCodigoQR(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            // Devuelve como data URL base64 para usarlo en el frontend directamente
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }
    }
    }
