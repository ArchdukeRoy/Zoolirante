using QRCoder;

namespace Zoolirante.Services
{
    public interface IQrCodeService
    {
        string GenerateQrCodeBase64(string ticketId);
    }

    public class QrCodeService : IQrCodeService
    {
        public string GenerateQrCodeBase64(string ticketId)
        {
            string qrContent = $"https://zoolirante.com/verify/{ticketId}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }
    }
}