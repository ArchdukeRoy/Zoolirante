using MailKit.Net.Smtp;
using MimeKit;

namespace Zoolirante.Services
{
    public interface IEmailService
    {
        Task SendTicketReceiptAsync(string toEmail, string customerName, List<TicketDetail> tickets, string sessionId);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IQrCodeService _qrCodeService;

        public EmailService(IConfiguration configuration, IQrCodeService qrCodeService)
        {
            _configuration = configuration;
            _qrCodeService = qrCodeService;
        }

        public async Task SendTicketReceiptAsync(string toEmail, string customerName, List<TicketDetail> tickets, string sessionId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zoolirante Zoo", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress(customerName, toEmail));
            message.Subject = "Your Zoolirante Ticket Receipt";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = BuildTicketReceiptHtml(customerName, tickets, sessionId);

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"],
                int.Parse(_configuration["Email:SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _configuration["Email:SmtpUser"],
                _configuration["Email:SmtpPassword"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private string BuildTicketReceiptHtml(string customerName, List<TicketDetail> tickets, string sessionId)
        {
            var total = tickets.Sum(t => t.Price);
            var receiptDate = DateTime.Now.ToString("MMMM dd, yyyy");

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2c5f2d; color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #e0e0e0; }}
        .ticket {{ background-color: white; border: 2px solid #2c5f2d; border-radius: 8px; padding: 20px; margin: 20px 0; }}
        .ticket-type {{ font-size: 20px; font-weight: bold; color: #2c5f2d; margin-bottom: 10px; }}
        .detail {{ margin: 8px 0; font-size: 14px; }}
        .label {{ font-weight: bold; color: #666; }}
        .price {{ font-size: 18px; color: #2c5f2d; font-weight: bold; margin-top: 15px; padding-top: 15px; border-top: 1px solid #e0e0e0; }}
        .qr-code {{ text-align: center; margin: 20px 0; padding: 20px; background-color: #f5f5f5; border-radius: 8px; }}
        .qr-code img {{ max-width: 200px; border: 3px solid #2c5f2d; border-radius: 8px; }}
        .ticket-number {{ text-align: center; font-family: monospace; font-size: 14px; color: #666; margin-top: 10px; background-color: #fff; padding: 10px; border-radius: 4px; }}
        .summary {{ background-color: white; padding: 20px; margin: 20px 0; border-radius: 8px; }}
        .total {{ font-size: 24px; font-weight: bold; color: #2c5f2d; text-align: right; padding-top: 15px; border-top: 2px solid #2c5f2d; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; padding: 20px; }}
        .info-box {{ background-color: #e8f5e9; border-left: 4px solid #2c5f2d; padding: 15px; margin: 20px 0; }}
        .receipt-id {{ color: #999; font-size: 12px; text-align: center; margin: 10px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🦁 Zoolirante Zoo</h1>
            <p style='margin: 10px 0 0 0; font-size: 18px;'>Ticket Receipt</p>
        </div>
        
        <div class='content'>
            <p style='font-size: 16px;'><strong>Dear {customerName},</strong></p>
            <p>Thank you for your purchase! Here are your tickets with QR codes for entry:</p>
            
            <div class='receipt-id'>Receipt ID: {sessionId}</div>
            <div class='receipt-id'>Date: {receiptDate}</div>
";

            foreach (var ticket in tickets)
            {
                var ticketId = ticket.TicketId.ToString();
                var qrCodeBase64 = _qrCodeService.GenerateQrCodeBase64(ticketId);

                html += $@"
            <div class='ticket'>
                <div class='ticket-type'>{ticket.Type}</div>
                <div class='detail'><span class='label'>📅 Date:</span> {ticket.Date}</div>
                <div class='detail'><span class='label'>🕐 Arrival Time:</span> {ticket.Time}</div>
                <div class='detail'><span class='label'>👥 Guests:</span> {ticket.Adults} Adult(s), {ticket.Children} Child(ren), {ticket.Concessions} Concession(s)</div>
                <div class='price'>Price: ${ticket.Price:F2}</div>
                
                <div class='qr-code'>
                    <p style='margin: 0 0 10px 0; font-weight: bold; color: #2c5f2d;'>📱 Scan this QR code at the entrance:</p>
                    <img src='data:image/png;base64,{qrCodeBase64}' alt='Ticket QR Code' />
                    <div class='ticket-number'>Ticket #: {ticketId}</div>
                </div>
            </div>
";
            }

            html += $@"
            <div class='summary'>
                <div class='total'>Total Paid: ${total:F2}</div>
            </div>
            
            <div class='info-box'>
                <p style='margin: 0; font-weight: bold;'>📌 Important Information:</p>
                <ul style='margin: 10px 0 0 0; padding-left: 20px;'>
                    <li><strong>Show the QR code above</strong> at the entrance for fast check-in</li>
                    <li>You can save this email or show it on your phone</li>
                    <li>Gates open 30 minutes before your scheduled arrival time</li>
                    <li>Free parking is included with your ticket</li>
                </ul>
            </div>
            
            <p style='margin-top: 30px; text-align: center;'>We look forward to seeing you at Zoolirante Zoo!</p>
        </div>
        
        <div class='footer'>
            <p><strong>Zoolirante Zoo</strong></p>
            <p>123 Zoo Lane, Adelaide, SA 5000</p>
            <p>📧 info@zoolirante.com | 📞 (08) 1234 5678</p>
        </div>
    </div>
</body>
</html>
";
            return html;
        }
    }

    public class TicketDetail
    {
        public int TicketId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Concessions { get; set; }
        public decimal Price { get; set; }
    }
}